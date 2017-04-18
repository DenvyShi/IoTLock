using IoTLock.IoT.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IoTLock.IoT.Picture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PictureView : Page
    {
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        private readonly string COGNITIVE_KEY = "a056eb1bd9cd4e2b9371c0a250af53cd";

        private const int BUTTON_PIN = 5;
        private GpioPin buttonPin;

        private MediaCapture mediaCapture;
        private StorageFile photoFile;

        public PictureView()
        {
            this.InitializeComponent();
            InitializeVideo();
            InitializeGPIO();

        }

        private async void InitializeVideo()
        {
            try
            {
                if (mediaCapture != null)
                {
                    // Cleanup MediaCapture object
                    //if (isPreviewing)
                    //{
                    //    await mediaCapture.StopPreviewAsync();
                    //    captureImage.Source = null;
                    //    playbackElement.Source = null;
                    //    isPreviewing = false;
                    //}
                    //if (isRecording)
                    //{
                    //    await mediaCapture.StopRecordAsync();
                    //    isRecording = false;
                    //    recordVideo.Content = "Start Video Record";
                    //    recordAudio.Content = "Start Audio Record";
                    //}
                    mediaCapture.Dispose();
                    mediaCapture = null;
                }

                statusLabel.Text = "Initializing camera to capture audio and video...";
                // Use default initialization
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                // Set callbacks for failure and recording limit exceeded
                statusLabel.Text = "Device successfully initialized for video recording!";
                //mediaCapture.Failed += new MediaCaptureFailedEventHandler(mediaCapture_Failed);
                //mediaCapture.RecordLimitationExceeded += new Windows.Media.Capture.RecordLimitationExceededEventHandler(mediaCapture_RecordLimitExceeded);

                // Start Preview
                previewElement.Source = mediaCapture;
                await mediaCapture.StartPreviewAsync();
                //isPreviewing = true;
                statusLabel.Text = "Camera preview succeeded";

                // Enable buttons for video and photo capture
                //SetVideoButtonVisibility(Action.ENABLE);

                // Enable Audio Only Init button, leave the video init button disabled
                //audio_init.IsEnabled = true;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Unable to initialize camera for audio/video mode: \n" + ex.Message;
            }
        }

        private void InitializeGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                statusLabel.Text = "There is no GPIO controller on this device.";
                return;
            }

            buttonPin = gpio.OpenPin(BUTTON_PIN);

            // Check if input pull-up resistors are supported
            if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonPin.SetDriveMode(GpioPinDriveMode.Input);

            // Set a debounce timeout to filter out switch bounce noise from a button press
            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            // Register for the ValueChanged event so our buttonPin_ValueChanged 
            // function is called when the button is pressed
            buttonPin.ValueChanged += ButtonPin_ValueChanged; ;

            statusLabel.Text = "GPIO pins initialized correctly.";
        }

        private async void ButtonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                await Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        var name = await CheckAuthorization();

                        if (name != null)
                        {
                            statusLabel.Text = name;
                        }
                    });
            }
        }

        private async Task<StorageFile> TakePicture()
        {
            try
            {
                photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    PHOTO_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        statusLabel.Text = "Photo taken";
                    });

                return photoFile;
                //IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.SetSource(photoStream);
            }
            catch (Exception ex)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        statusLabel.Text = ex.Message;
                    });
                return null;
            }
        }

        private async Task<string> CheckAuthorization()
        {
            try
            {
                FaceServiceHelper.ApiKey = COGNITIVE_KEY;

                var picture = await TakePicture();

                var personsGroups = await FaceServiceHelper.GetPersonGroupsAsync();
                if (personsGroups == null)
                    return null;

                var gabGroup = personsGroups.FirstOrDefault(p => p.PersonGroupId == "2");

                var results = await FaceServiceHelper.DetectAsync(delegate { return picture.OpenStreamForReadAsync(); },
                    returnFaceId: true);
                if (results.Count() > 0)
                {
                    var faceId = results.FirstOrDefault().FaceId;
                    var identityResult = await FaceServiceHelper.IdentifyAsync("2", new Guid[] { faceId });

                    var personId = identityResult.FirstOrDefault().Candidates
                        .OrderBy(p => p.Confidence)
                        .FirstOrDefault()
                        .PersonId;

                    var personsInGabGroup = await FaceServiceHelper.GetPersonsAsync("2");
                    var authorized = personsInGabGroup.Any(p => p.PersonId == personId);

                    if (authorized)
                    {
                        var person = personsInGabGroup.FirstOrDefault(p => p.PersonId == personId);
                        return person.Name;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () =>
                    {
                        statusLabel.Text = ex.Message;
                    });
                return null;
            }
        }
    }
}
