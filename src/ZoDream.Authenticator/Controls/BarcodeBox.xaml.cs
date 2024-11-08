using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using ZXing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ZoDream.Authenticator.Controls
{
    public sealed partial class BarcodeBox : UserControl
    {
        public BarcodeBox()
        {
            this.InitializeComponent();
            
            Loaded += BarcodeBox_Loaded;
            Unloaded += BarcodeBox_Unloaded;
        }

        //private MediaFrameSourceGroup? _mediaFrameSourceGroup;
        private MediaCapture? _mediaCapture;
        private MediaFrameReader? _frameReader;
        private readonly BarcodeReader<SoftwareBitmap> _reader = new(o => new SoftwareBitmapLuminanceSource(o));

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BarcodeBox), new PropertyMetadata(string.Empty));

        public event TextChangedEventHandler? TextChanged;

        public void Start()
        {
            if (_mediaCapture is not null)
            {
                return;
            }
            StartCaptureElement();
        }

        public void Stop()
        {
            _mediaCapture?.Dispose();
            if (_frameReader != null)
            {
                _frameReader.FrameArrived -= FrameReader_FrameArrived;
                _frameReader.Dispose();
                _frameReader = null;
            }
            _mediaCapture = null;
            captureElement.Source = null;
            captureElement.SetMediaPlayer(null);
        }

        private async void StartCaptureElement()
        {
            var groups = await MediaFrameSourceGroup.FindAllAsync();
            if (groups.Count == 0)
            {
                tbkTip.Text = "No camera devices found.";
                return;
            }
            var mediaFrameSourceGroup = groups.First();
            //var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            //mediaFrameSourceGroup = groups.Where(g => g.SourceInfos.Any(s => s.SourceKind == MediaFrameSourceKind.Color &&
            //                                                                (s.MediaStreamType == MediaStreamType.VideoPreview || s.MediaStreamType == MediaStreamType.VideoRecord))
            //                                                                && g.SourceInfos.All(sourceInfo => videoDevices.Any(vd => vd.Id == sourceInfo.DeviceInformation.Id))).First();

            _mediaCapture = new MediaCapture();
            var mediaCaptureInitializationSettings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = mediaFrameSourceGroup,
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };
            await _mediaCapture.InitializeAsync(mediaCaptureInitializationSettings);

            // Set the MediaPlayerElement's Source property to the MediaSource for the mediaCapture.
            var frameSource = _mediaCapture.FrameSources[mediaFrameSourceGroup.SourceInfos[0].Id];
            if (frameSource is null)
            {
                return;
            }
            captureElement.Source = Windows.Media.Core.MediaSource.CreateFromMediaFrameSource(frameSource);
            _frameReader = await _mediaCapture.CreateFrameReaderAsync(frameSource);
            if (_frameReader is null)
            {
                return;
            }
            _frameReader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;
            _frameReader.FrameArrived += FrameReader_FrameArrived;
            await _frameReader.StartAsync();
        }

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var frame = sender.TryAcquireLatestFrame();
            var softwareBitmap = frame.VideoMediaFrame.GetVideoFrame()
                .SoftwareBitmap;
            var res = _reader.Decode(softwareBitmap);
            if (res is null)
            {
                return;
            }
            DispatcherQueue.TryEnqueue(() => {
                Text = res.Text;
                TextChanged?.Invoke(this, TextChangedEventArgs.FromAbi(IntPtr.Zero));
            });
            //DispatcherQueue.TryEnqueue(() => {
            //    var bitmap = new WriteableBitmap(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);
            //    softwareBitmap.CopyToBuffer(bitmap.PixelBuffer);
            //    var res = _reader.Decode(bitmap.ToSKBitmap());
            //    if (res is null)
            //    {
            //        return;
            //    }
            //    Text = res.Text;
            //    TextChanged?.Invoke(this, TextChangedEventArgs.FromAbi(IntPtr.Zero));
            //});
        }

        private async void CapturePhoto_Click(object sender, RoutedEventArgs e)
        {
            // Capture a photo to a stream
            var imgFormat = ImageEncodingProperties.CreateJpeg();
            var stream = new InMemoryRandomAccessStream();
            await _mediaCapture?.CapturePhotoToStreamAsync(imgFormat, stream);
            stream.Seek(0);

            // Show the photo in an Image element
            var bmpImage = new BitmapImage();
            await bmpImage.SetSourceAsync(stream);
        }

        private void BarcodeBox_Loaded(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void BarcodeBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }
    }
}
