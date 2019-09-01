using System.Threading.Tasks;
using System.Windows.Input;

namespace TraderSys.FullStockTicker.ClientApp
{
    public interface IAsyncCommand : ICommand
    {
        bool CanExecute();
        Task ExecuteAsync();
        void RaiseCanExecuteChanged();
    }
}