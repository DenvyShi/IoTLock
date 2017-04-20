using IoTLock.Mobile.Helpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IoTLock.Mobile.ViewModels
{
    public class CadastroViewModel: BaseViewModel
    {   
        //Services
        private MediaFile _image;
        private readonly Services.IMvvmService _mvvmService;

        //Commands
        public Command GrupoCommand { get; set; }
        public Command CadastrarPessoaCommand { get; set; }
        public Command ConfirmedCommand { get; set; }


        //Bindings
        private string _textoGrupo;
        public string TextoGrupo
        {
            get { return _textoGrupo; }
            set { _textoGrupo = value; OnPropertyChanged(); }
        }


        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; OnPropertyChanged(); }
        }

        private string _textoPessoa;
        public string TextoPessoa
        {
            get { return _textoPessoa; }
            set { _textoPessoa = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PersonGroupOveridden> _grupos;
        public ObservableCollection<PersonGroupOveridden> Grupos
        {
            get { return _grupos; }
            set { _grupos = value; OnPropertyChanged(); }
        }
        public Command TakePictureCommand { get; set; }

        private PersonGroupOveridden _selectedGroup;
        public PersonGroupOveridden SelectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup = value; OnPropertyChanged(); RefreshPeople(); }
        }

        private ObservableCollection<PeopleOveridden> _people;
        public ObservableCollection<PeopleOveridden> People
        {
            get { return _people; }
            set { _people = value; OnPropertyChanged(); }
        }

        private PeopleOveridden _selectedPerson;
        public PeopleOveridden SelectedPerson
        {
            get { return _selectedPerson; }
            set { _selectedPerson = value; OnPropertyChanged(); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; OnPropertyChanged(); }
        }



        //Construtor
        public CadastroViewModel()
        {
            _mvvmService = DependencyService.Get<Services.IMvvmService>();
            FaceServiceHelper.ApiKey = "a056eb1bd9cd4e2b9371c0a250af53cd";
            GrupoCommand = new Command(ExecuteGrupoCommand);
            TakePictureCommand = new Command(ExecuteTakePictureCommand);
            CadastrarPessoaCommand = new Command(ExecuteCadastrarPessoaCommand);
            ConfirmedCommand = new Command(ExecuteConfirmedCommand);
            RefreshGroups();
        }

        public async Task<Stream> GetStream()
        {
            return await Task.Factory.StartNew<Stream>(() =>
            {
                var stream = _image.GetStream();
                Stream newStream;
                newStream = stream;
                return newStream;
            });
        }

        private async void ExecuteConfirmedCommand()
        {
            try
            {
                IsRunning = true;
                var result = await FaceServiceHelper.DetectAsync(GetStream, true);
                if (result == null || result.Count() == 0)
                    return;
                await FaceServiceHelper.AddPersonFaceAsync(SelectedGroup.PersonGroupId, SelectedPerson.PersonId, GetStream,
                    null, result.FirstOrDefault().FaceRectangle);
                await FaceServiceHelper.TrainPersonGroupAsync(SelectedGroup.PersonGroupId);
                ImageUrl = string.Empty;
                await _mvvmService.MessageMvvm("Mensagem", "Rosto cadastrado com sucesso", "Ok");
            }
            catch (Exception ex) { }
            finally
            {
                IsRunning = false;
            }
        }

        private async void ExecuteCadastrarPessoaCommand()
        {
            try
            {
                await FaceServiceHelper.CreatePersonAsync(SelectedGroup.PersonGroupId, TextoPessoa);
                RefreshPeople();
                TextoPessoa = string.Empty;
            }
            catch (Exception ex) {}
        }

        private async void ExecuteGrupoCommand()
        {
            try
            {
                await FaceServiceHelper.CreatePersonGroupAsync(Guid.NewGuid().ToString(), TextoGrupo, null);
                RefreshGroups();
                TextoGrupo = string.Empty;
            }
            catch (Exception ex) {}
        }

        private async void ExecuteTakePictureCommand()
        {
            await CrossMedia.Current.Initialize();
            _image = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions());
            ImageUrl = _image?.Path;
        }

        private async void RefreshGroups()
        {
            try
            {
                var result = await FaceServiceHelper.GetPersonGroupsAsync();
                Grupos = new ObservableCollection<PersonGroupOveridden>();
                foreach (var item in result)
                {
                    Grupos.Add(new PersonGroupOveridden
                    {
                        Name = item.Name,
                        PersonGroupId = item.PersonGroupId,
                        UserData = item.UserData
                    });
                }
            }
            catch (Exception ex) {}
        }

        private async void RefreshPeople()
        {
            try
            {
                if (SelectedGroup == null)
                    return;
                var result = await FaceServiceHelper.GetPersonsAsync(SelectedGroup.PersonGroupId);
                People = new ObservableCollection<PeopleOveridden>();

                foreach (var item in result)
                {
                    People.Add(new PeopleOveridden
                    {
                        Name = item.Name,
                        PersistedFaceIds = item.PersistedFaceIds,
                        PersonId = item.PersonId,
                        UserData = item.UserData
                    });
                }
            }
            catch (Exception ex) {}
        }
    }
}
