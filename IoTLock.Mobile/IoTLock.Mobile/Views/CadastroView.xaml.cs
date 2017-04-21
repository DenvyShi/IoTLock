
using IoTLock.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IoTLock.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CadastroView : TabbedPage
    {
        public CadastroView()
        {
            InitializeComponent();
            BindingContext = new CadastroViewModel();
        }
    }
}
