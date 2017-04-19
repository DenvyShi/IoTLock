using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IoTLock.Mobile.ViewModels
{
    public class CadastroViewModel: BaseViewModel
    {
        //Bindings
        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; OnPropertyChanged(); }
        }

        public Command TakePictureCommand { get; set; }

        //Construtor
        public CadastroViewModel()
        {
            TakePictureCommand = new Command(ExecuteTakePictureCommand);
        }

        private async void ExecuteTakePictureCommand()
        {
            await CrossMedia.Current.Initialize();
            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());
            ImageUrl = mediaFile?.Path;
        }
    }
}
