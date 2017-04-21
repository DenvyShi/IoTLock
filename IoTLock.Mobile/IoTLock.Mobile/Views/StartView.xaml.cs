using IoTLock.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IoTLock.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartView : ContentPage
    {
        public StartView()
        {
            InitializeComponent();
            BindingContext = new StartViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
