using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HiddenMetroCamera
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Windows.Media.Capture.MediaCapture m_mediaCaptureMgr;
        private Windows.Storage.StorageFile m_photoStorageFile;
        private Windows.Storage.StorageFile m_recordStorageFile;
        private bool initialized = false;
        private bool debug = true;//wanna debug? set the statusListBox to Visible

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        internal async Task initializeCamera()
        {
            try
            {
                if (!initialized)
                {
	                m_mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
	                await m_mediaCaptureMgr.InitializeAsync();
                    if (debug) statusListBox.Items.Add("Device initialized successful");                    
                    initialized = true;
                }
            }
            catch (Exception exception)
            {
                if (debug) statusListBox.Items.Add("error initialize");
            }
        }

        internal async void startButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await initializeCamera();
                string PHOTO_FILE_NAME = string.Format("img-{0:yyyy-MM-dd_hh-mm-ss-tt}.jpg", DateTime.Now);
                m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await m_mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);
                if (debug) statusListBox.Items.Add(PHOTO_FILE_NAME + " saved");
            }
            catch (Exception exception)
            {
                if (debug) statusListBox.Items.Add("error occurred");
            }
        }
    }
}
