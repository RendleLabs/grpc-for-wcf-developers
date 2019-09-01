using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace TraderSys.FullStockTicker.ClientApp
{
    /// <summary>
    /// Interaction logic foApp_OnStartup  /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddGrpcClient<FullStockTickerServer.Protos.FullStockTicker.FullStockTickerClient>(options =>
            {
                options.Address = new Uri("https://localhost:5001");
            });
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var window = _serviceProvider.GetService<MainWindow>();
            window.Show();
        }
    }
}
