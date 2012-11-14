using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Media.MediaProperties;

namespace HiddenDesktopCamera
{
    public partial class Form1 : Form
    {
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private Windows.Storage.StorageFile m_photoStorageFile;
        private Windows.Storage.StorageFile m_recordStorageFile;
        private bool initialized = false;

        public Form1()
        {
            InitializeComponent();
        }

        internal async void initializeCamera()
        {
            try
            {
                if (!initialized)
                {
                    m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                    await m_mediaCaptureMgr.InitializeAsync();
                    statusListBox.Items.Add("Device initialized successful");
                    initialized = true;
                }
            }
            catch (Exception exception)
            {
                statusListBox.Items.Add("error initialize");
            }
        }

        internal async void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                initializeCamera();
                string PHOTO_FILE_NAME = string.Format("img-{0:yyyy-MM-dd_hh-mm-ss-tt}.jpg", DateTime.Now);
                m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);
                statusListBox.Items.Add(PHOTO_FILE_NAME + " saved");
            }
            catch (Exception exception)
            {
                statusListBox.Items.Add("still initializing...");
            }
        }
    }
}
