using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using TraderSys.FullStockTicker.ClientApp.Annotations;
using TraderSys.FullStockTickerServer.Protos;

namespace TraderSys.FullStockTicker.ClientApp
{
    public class MainWindowViewModel : IAsyncDisposable, INotifyPropertyChanged
    {
        private readonly FullStockTickerServer.Protos.FullStockTicker.FullStockTickerClient _client;
        private readonly IClientStreamWriter<ActionMessage> _requestStream;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _responseTask;
        private string _addSymbol;

        public MainWindowViewModel(FullStockTickerServer.Protos.FullStockTicker.FullStockTickerClient client)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _client = client;
            var duplexStream = _client.Subscribe();
            _requestStream = duplexStream.RequestStream;
            _responseTask = HandleResponsesAsync(duplexStream.ResponseStream, _cancellationTokenSource.Token);
            AddCommand = new AsyncCommand(Add, CanAdd);
        }
        
        public IAsyncCommand AddCommand { get; }

        public string AddSymbol
        {
            get => _addSymbol;
            set
            {
                if (value == _addSymbol) return;
                _addSymbol = value;
                AddCommand.RaiseCanExecuteChanged();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PriceViewModel> Prices { get; } = new ObservableCollection<PriceViewModel>();

        private async Task Add()
        {
            if (CanAdd())
            {
                await _requestStream.WriteAsync(new ActionMessage {Add = new AddSymbolRequest {Symbol = AddSymbol}});
            }
        }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(AddSymbol);

        public async Task Remove(PriceViewModel priceViewModel)
        {
            await _requestStream.WriteAsync(new ActionMessage {Remove = new RemoveSymbolRequest {Symbol = priceViewModel.Symbol}});
            Prices.Remove(priceViewModel);
        }

        private async Task HandleResponsesAsync(IAsyncStreamReader<StockTickerUpdate> stream, CancellationToken token)
        {
            while (await stream.MoveNext(token))
            {
                var update = stream.Current;
                var price = Prices.FirstOrDefault(p => p.Symbol.Equals(update.Symbol));
                if (price == null)
                {
                    price = new PriceViewModel(this) {Symbol = update.Symbol, Price = Convert.ToDecimal(update.Price)};
                    Prices.Add(price);
                }
                else
                {
                    price.Price = Convert.ToDecimal(update.Price);
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            return _responseTask.IsCompleted ? default : new ValueTask(_responseTask);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}