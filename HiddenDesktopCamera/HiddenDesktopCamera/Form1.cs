using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
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
        globalKeyboardHook gkh = new globalKeyboardHook();

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

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;

                //this is probably ignored anyway since it's too small, 
                //and will be overrided by a minimum value by the OS
                notifyIcon1.ShowBalloonTip(200);

                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gkh.HookedKeys.Add(Keys.A);
            gkh.HookedKeys.Add(Keys.PrintScreen);
            //gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            //statusListBox.Items.Add("Up\t" + e.KeyCode.ToString());
            statusListBox.Items.Add("fired");
            e.Handled = true;
        }

        //void gkh_KeyDown(object sender, KeyEventArgs e)
        //{
        //    statusListBox.Items.Add("Down\t" + e.KeyCode.ToString());
        //    e.Handled = true;
        //} 
    }
}
