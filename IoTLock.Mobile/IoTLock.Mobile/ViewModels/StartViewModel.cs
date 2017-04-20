using System;
using IoTLock.Mobile.Helpers;
using Plugin.Media;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

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
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; OnPropertyChanged(); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; OnPropertyChanged(); }
        }



        //Services
        private readonly Services.IMvvmService _mvvmService;

        //Commands
        public Command CheckFaceCommand { get; set; }
        public Command FaceConfirmerdCommand { get; set; }

        //Construtor
        public StartViewModel()
        {
            FaceServiceHelper.ApiKey = "a056eb1bd9cd4e2b9371c0a250af53cd";
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
            IsRunning = true;
            await CrossMedia.Current.Initialize();
            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());
            ImageUrl = mediaFile?.Path;

            try
            {
                var func = new Func<Task<Stream>>(() => Task.Factory.StartNew<Stream>(() => mediaFile.GetStream()));
                var result = await FaceServiceHelper.DetectAsync(func, true, false, null);
                var resultIdentify = await FaceServiceHelper.IdentifyAsync("2", result.Select(p => p.FaceId).ToArray());
                if (resultIdentify.Any())
                {
                    var candidate = resultIdentify.FirstOrDefault().Candidates
                        .OrderBy(p => p.Confidence)
                        .FirstOrDefault();
                    StatusResult = "Rosto Cadastrado";
                    TextColor = Color.Green;

                    var person = await FaceServiceHelper.GetPersonAsync("2", candidate.PersonId);
                    Text = person.Name;
                    IsChecked = true;
                }
                else
                {
                    StatusResult = "Rosto Não Cadastrado";
                    TextColor = Color.Red;
                    IsChecked = false;
                }                
            }
            catch (Exception ex)
            {
                await _mvvmService.MessageMvvm("Error", ex.Message, "ok");
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
