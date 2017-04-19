using System;
using Plugin.Media;
using Xamarin.Forms;

namespace IoTLock.Mobile.ViewModels
{
    public class StartViewModel: BaseViewModel
    {
        //Bindings
        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; OnPropertyChanged(); }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; OnPropertyChanged(); }
        }
        private string _statusResult;
        public string StatusResult
        {
            get { return _statusResult; }
            set { _statusResult = value; OnPropertyChanged(); }
        }
        private Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set { _textColor = value; OnPropertyChanged(); }
        }

        //Services
        private readonly Services.IMvvmService _mvvmService;

        //Commands
        public Command CheckFaceCommand { get; set; }
        public Command FaceConfirmerdCommand { get; set; }

        //Construtor
        public StartViewModel()
        {
            Text = "Welcome";
            StatusResult = string.Empty;
            _mvvmService = DependencyService.Get<Services.IMvvmService>();
            CheckFaceCommand = new Command(ExecuteCheckFaceCommand);
            FaceConfirmerdCommand = new Command(ExecuteFaceConfirmerdCommand);
        }

        private void ExecuteFaceConfirmerdCommand()
        {
            _mvvmService.PushNavigationMvvm(new Views.HomeView());
        }

        private async void ExecuteCheckFaceCommand()
        {
            await CrossMedia.Current.Initialize();
            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());
            ImageUrl = mediaFile?.Path;
            StatusResult = "Rosto cadastrado";
            TextColor = Color.Green;
            Text = "Jonathan Braga";            
        }
    }
}
