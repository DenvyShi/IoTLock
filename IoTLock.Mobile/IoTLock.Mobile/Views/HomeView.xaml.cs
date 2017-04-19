using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IoTLock.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            if(e.Value)
            {
                DisplayAlert("Title", "Ativado", "Ok");
            }
            else
            {
                DisplayAlert("Title", "Desativado", "Ok");
            }
            
        }
    }
}
