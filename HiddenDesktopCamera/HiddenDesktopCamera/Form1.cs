using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using Windows.Foundation;
using Windows.Media.MediaProperties;

namespace HiddenDesktopCamera {
    public partial class Form1 : Form {
        
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private Windows.Storage.StorageFile m_photoStorageFile;

        //is the camera initialized?
        private bool initialized = false;

        //set to true for debug mode
        private bool debug = false;

        //create a global keyboard hook
        globalKeyboardHook gkh = new globalKeyboardHook();

       //public delegate void captureImage();

        public Form1() {
            InitializeComponent();
        }

        //initialize the camera if it's not already initialized
        internal async Task initializeCamera() {
            try {
                if (!initialized) {
                    m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                    await m_mediaCaptureMgr.InitializeAsync();
                    statusListBox.Invoke(new MethodInvoker(() => statusListBox.Items.Add("Device initialized successful")));
                    initialized = true;
                }
            } 
            catch (Exception exception) {
                if (debug) statusListBox.Items.Add(exception);
                else statusListBox.Invoke(new MethodInvoker(() => statusListBox.Items.Add("Error initializing camera")));
            }
        }

        internal async void startButton_Click(object sender, EventArgs e) {
            await captureImage();
        }

        async private Task captureImage() {
            try {

                //initialize the camera
                await initializeCamera();

                //capture the image
                string PHOTO_FILE_NAME = string.Format("img-{0:yyyy-MM-dd_hh-mm-ss-tt}.jpg", DateTime.Now);
                m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                //write to the status box
                statusListBox.Invoke(new MethodInvoker(() => statusListBox.Items.Add(PHOTO_FILE_NAME + " saved")));
            } catch (Exception exception) {
                if (debug) statusListBox.Items.Add(exception);
                else statusListBox.Invoke(new MethodInvoker(() => statusListBox.Items.Add("Error capturing image...")));
            }
        }

        private void Form1_Resize(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                notifyIcon1.Visible = true;

                //this is probably ignored anyway since it's too small, 
                //and will be overrided by a minimum value by the OS
                //notifyIcon1.ShowBalloonTip(200);

                this.ShowInTaskbar = false;
            }
        }

        //restore the form when double click on the tray icon
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            //notifyIcon1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e) {
            gkh.HookedKeys.Add(Keys.PrintScreen);
            gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
        }

        void gkh_KeyUp(object sender, KeyEventArgs e) {
            try {
                if (debug) statusListBox.Items.Add("Up\t" + e.KeyCode.ToString());

                Thread oThread = new Thread(new ThreadStart(async () => await this.captureImage()));

                // Start the thread
                oThread.Start();

                //handle the keys event so that no other apps can react to it
                e.Handled = true;
            } catch (Exception) {                
                statusListBox.Invoke(new MethodInvoker(() => statusListBox.Items.Add("Error capturing image...")));
            }
        }
    }
}
