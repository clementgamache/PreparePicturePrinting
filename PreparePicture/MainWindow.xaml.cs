using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Drawing.Imaging;

namespace PreparePicture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private FolderBrowserDialog folderDialog = new FolderBrowserDialog();
        private OpenFileDialog fileDialog = new OpenFileDialog();
        private string folderName;
        private string fileName;
        double width, height, mirror, xMargin, yMargin;
        int pixelsToRemove;
        ImageFormat imgFormat;
        bool folderSelected;
        private int numberOfMirrors;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void showFolderDialog(object sender, RoutedEventArgs e)
        {
            folderDialog.ShowDialog();
            textBoxFolder.Text = folderDialog.SelectedPath;
        }

        private void showFileDialog(object sender, RoutedEventArgs e)
        {
            fileDialog.ShowDialog();
            textBoxFile.Text = fileDialog.FileName;
        }

        private void start(object sender, RoutedEventArgs e)
        {
            folderName = textBoxFolder.Text;
            fileName = textBoxFile.Text;

            width=Convert.ToDouble(textBoxWidth.Text);
            height=Convert.ToDouble(textBoxHeight.Text);
            mirror=Convert.ToDouble(textBoxMirror.Text);
            numberOfMirrors = Convert.ToInt32(textBoxNumberOfMirrors.Text);
            xMargin=Convert.ToDouble(textBoxHorizontalMargins.Text);
            yMargin=Convert.ToDouble(textBoxVerticalMargins.Text);
            pixelsToRemove = Convert.ToInt32(textBoxPixelsToRemoveOnEachSide.Text);
            switch (textBoxImageType.Text)
            {
                case "jpg":
                case "jpeg":
                    imgFormat = ImageFormat.Jpeg;
                    break;
                case "tiff":
                case "tif":
                    imgFormat = ImageFormat.Tiff;
                    break;
                case "bmp":
                    imgFormat = ImageFormat.Bmp;
                    break;
                case "png":
                    imgFormat = ImageFormat.Png;
                    break;


            }
            if (tabItemFolder.IsSelected)
            {

                folderSelected = true;
            }
            else if (tabItemFile.IsSelected)
            {
                folderSelected = false;
            }
            else
            {
                throw new Exception("No tab selected");
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string[] files = new string[] { };
                if (folderSelected)
                {

                    files = Directory.GetFiles(folderName);
                }
                else
                {
                    files = new string[] { fileName };
                }

                int i = 0;
                foreach (string file in files)
                {

                    (sender as BackgroundWorker).ReportProgress(i * 100 / files.Length);

                    ImageToPrint img = new ImageToPrint(file,
                        width,
                        height,
                        mirror,
                        numberOfMirrors,
                        xMargin,
                        yMargin,
                        pixelsToRemove,
                        imgFormat);
                    img.createFileToPrint();
                    i++;

                }
            (sender as BackgroundWorker).ReportProgress(i * 100 / files.Length);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
            if (pbStatus.Value < 100) {
                buttonStart.IsEnabled = false;
            }
            else
            {
                buttonStart.IsEnabled = true;
                System.Windows.MessageBox.Show("Done.");
            }
        }

        /*void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            

            
        }
        
        */
    }
}
