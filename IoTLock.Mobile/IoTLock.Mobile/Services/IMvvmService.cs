using System.Threading.Tasks;
using Xamarin.Forms;

namespace IoTLock.Mobile.Services
{
    public interface IMvvmService
    {
        Task PushNavigationMvvm(Page page);
        Task MessageMvvm(string title, string message, string buttonTitle);
    }
}
