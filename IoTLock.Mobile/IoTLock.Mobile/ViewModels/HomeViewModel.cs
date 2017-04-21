using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IoTLock.Mobile.ViewModels
{
    public class HomeViewModel: BaseViewModel
    {
        //Services
        private readonly Services.IMvvmService _navigationMvvm;

        //Commands
        public Command ConfigCommand { get; set; }

        //Construtor
        public HomeViewModel()
        {
            _navigationMvvm = DependencyService.Get<Services.IMvvmService>();
            ConfigCommand = new Command(ExecuteConfigCommand);
        }

        private void ExecuteConfigCommand()
        {
            _navigationMvvm.PushNavigationMvvm(new Views.CadastroView());
        }
    }
}
