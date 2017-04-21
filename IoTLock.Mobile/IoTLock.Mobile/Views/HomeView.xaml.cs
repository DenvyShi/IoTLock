using IoTLock.Mobile.ViewModels;
using Microsoft.AspNet.SignalR.Client;
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
        IHubProxy lightingHubProxy;

        public HomeView()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            BindingContext = new HomeViewModel();

            var hubConnection = new HubConnection("http://iotlock-api.azurewebsites.net");
            lightingHubProxy = hubConnection.CreateHubProxy("LightingHub");
            hubConnection.Start().Wait();
        }

        private async void Corredor_Changed(object sender, ToggledEventArgs e)
        {
            if (lightingHubProxy != null)
                await lightingHubProxy.Invoke("toggle", "hallway");
        }

        private async void Cozinha_Changed(object sender, ToggledEventArgs e)
        {
            if (lightingHubProxy != null)
                await lightingHubProxy.Invoke("toggle", "kitchen");
        }

        private async void Jardim_Changed(object sender, ToggledEventArgs e)
        {
            if (lightingHubProxy != null)
                await lightingHubProxy.Invoke("toggle", "garden");
        }

        private async void Quarto_Changed(object sender, ToggledEventArgs e)
        {
            if (lightingHubProxy != null)
                await lightingHubProxy.Invoke("toggle", "bedroom");
        }
    }
}
