using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreparePicture
{
    class ImageToPrint
    {
        private string path;
        private Image image;
        private System.Windows.Size sizeIn;
        private double mirrorIn;
        private bool isVertical;
        private double horizontalMarginsIn, verticalMarginsIn;
        private int pixelsToRemove;
        private ImageFormat imgFormat;
        private int numberOfMirrors;

        public ImageToPrint(string path, double widthIn, double heightIn, double mirrorIn, int numberOfMirrors, double horizontalWhiteSpaceIn, double verticalWhiteSpaceIn, int pixelsToRemove, ImageFormat imgFormat)
        {
            this.path = path;
            image = Image.FromFile(path);
            isVertical = image.Height > image.Width;
            double min = Math.Min(heightIn, widthIn);
            double max = Math.Max(heightIn, widthIn);
            if (isVertical)
            {
                sizeIn = new System.Windows.Size(min, max);
            }
            else
            {
                sizeIn = new System.Windows.Size(max, min);
            }
            this.mirrorIn = mirrorIn;
            horizontalMarginsIn = horizontalWhiteSpaceIn;
            verticalMarginsIn = verticalWhiteSpaceIn;
            this.pixelsToRemove = pixelsToRemove;
            this.imgFormat = imgFormat;
            this.numberOfMirrors = numberOfMirrors;
        }

        private Size getPixelSize()
        {
            int pixelsX = (int)((mirrorIn + horizontalMarginsIn * 2 + sizeIn.Width) * (image.Width-2*pixelsToRemove) / sizeIn.Width);
            int pixelsY = (int)((mirrorIn + verticalMarginsIn * 2 + sizeIn.Height) * (image.Height-2*pixelsToRemove) / sizeIn.Height);
            return new Size(pixelsX, pixelsY);
        }

        public void createFileToPrint()
        {
            Bitmap bmp = new Bitmap(image);
            Rectangle cropRect = new Rectangle(pixelsToRemove,pixelsToRemove,bmp.Width-pixelsToRemove*2,bmp.Height-pixelsToRemove*2);
            Bitmap cropped = new Bitmap(cropRect.Width, cropRect.Height);


            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(bmp, new Rectangle(0, 0, cropped.Width, cropped.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            Utilities.addMirror(ref cropped, sizeIn.Width, mirrorIn, numberOfMirrors);
            Utilities.addMargins(ref cropped, sizeIn.Width, horizontalMarginsIn, verticalMarginsIn);
            float dpi = ((float)cropped.Width) / ((float)(sizeIn.Width + 2 * mirrorIn + 2 * horizontalMarginsIn));
            cropped.SetResolution(dpi, dpi);
            string folderName = Path.GetDirectoryName(path);
            Directory.CreateDirectory(folderName + @"\modified\"); 
            string newName = folderName + @"\modified\" + System.IO.Path.GetFileNameWithoutExtension(path) + " " + sizeIn.Width + "x" + sizeIn.Height + "M";
            if (System.IO.File.Exists(newName))
                System.IO.File.Delete(newName);
            string extension;
            if (imgFormat == ImageFormat.Jpeg)
            {
                extension = "jpg";
            }
            else if (imgFormat == ImageFormat.Tiff)
            {
                extension = "tiff";
            }
            else if (imgFormat == ImageFormat.Png)
            {
                extension = "png";
            }
            else if (imgFormat == ImageFormat.Bmp)
            {
                extension = "bmp";
            }
            else
            {
                imgFormat = ImageFormat.Jpeg;
                extension = "jpg";
            }
            newName = Path.ChangeExtension(newName, "." + extension);
            cropped.Save(newName, imgFormat);
            cropped.Dispose();
            bmp.Dispose();
            image.Dispose();
        }

    }
}
