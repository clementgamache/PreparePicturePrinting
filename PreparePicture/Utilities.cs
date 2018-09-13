using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PreparePicture
{
    class Utilities
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static void ResizeImage(ref Bitmap bmp, int width, int height)
        {
            Bitmap backup = (Bitmap)bmp.Clone();
            var destRect = new Rectangle(0, 0, width, height);
            bmp = new Bitmap(width, height);

            bmp.SetResolution(backup.HorizontalResolution, backup.VerticalResolution);

            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(backup, destRect, 0, 0, backup.Width, backup.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }

        public static void addMirror(ref Bitmap bmp, double imageWidthIn, double mirrorWidthIn, int numberOfMirrors)
        {
            int mirrorWidthPx = (int)(bmp.Width * mirrorWidthIn / imageWidthIn);
            addMirror(ref bmp, mirrorWidthPx, numberOfMirrors);
        }

        public static void addMirror(ref Bitmap bmp, int mirrorWidthPx, int numberOfMirrors)
        {
            if (mirrorWidthPx > 0)
            {
                for (int i = 0; i < numberOfMirrors; i++)
                {
                    Bitmap backup = (Bitmap)bmp.Clone();
                    bmp.Dispose();
                    bmp = new Bitmap(backup.Width + mirrorWidthPx * 2, backup.Height + mirrorWidthPx * 2);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawImage(backup, mirrorWidthPx, mirrorWidthPx);
                        backup.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        g.DrawImage(backup, mirrorWidthPx - backup.Width, mirrorWidthPx);
                        g.DrawImage(backup, backup.Width + mirrorWidthPx, mirrorWidthPx);
                        backup.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        g.DrawImage(backup, mirrorWidthPx - backup.Width, mirrorWidthPx - backup.Height);
                        g.DrawImage(backup, mirrorWidthPx + backup.Width, mirrorWidthPx - backup.Height);
                        g.DrawImage(backup, mirrorWidthPx - backup.Width, mirrorWidthPx + backup.Height);
                        g.DrawImage(backup, mirrorWidthPx + backup.Width, mirrorWidthPx + backup.Height);
                        backup.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        g.DrawImage(backup, mirrorWidthPx, mirrorWidthPx - backup.Height);
                        g.DrawImage(backup, mirrorWidthPx, mirrorWidthPx + backup.Height);
                    }
                    backup.Dispose();
                }
            }
        }


        public static void addMargins(ref Bitmap bmp, double imageWidthIn, double horizontalMarginsIn, double verticalMarginsIn)
        {
            int verticalPx = (int)(verticalMarginsIn * bmp.Width / imageWidthIn);
            int horizontalPx = (int)(horizontalMarginsIn * bmp.Width / imageWidthIn);
            addMargins(ref bmp, verticalPx, horizontalPx);
        }

        public static void addMargins(ref Bitmap bmp, int horizontalMarginsPx, int verticalMarginsPx)
        {
            if (horizontalMarginsPx > 0 || verticalMarginsPx > 0)
            {
                Bitmap backup = (Bitmap)bmp.Clone();
                bmp.Dispose();
                bmp = new Bitmap(backup.Width + horizontalMarginsPx * 2, backup.Height + verticalMarginsPx * 2);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, bmp.Width, bmp.Height));
                    g.DrawImage(backup, horizontalMarginsPx, verticalMarginsPx);
                }
                backup.Dispose();
            }
        }
    }
}
