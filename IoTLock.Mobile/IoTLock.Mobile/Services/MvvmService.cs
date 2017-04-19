using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IoTLock.Mobile.Services
{
    public class MvvmService : IMvvmService
    {
        public async Task MessageMvvm(string title, string message, string buttonTitle)
        {
            await App.Current.MainPage.DisplayAlert(title, message, buttonTitle);
        }

        public async Task PushNavigationMvvm(Page page)
        {
            await App.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}
