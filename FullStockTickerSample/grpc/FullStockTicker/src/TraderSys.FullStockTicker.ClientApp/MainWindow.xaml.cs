namespace TraderSys.FullStockTicker.ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _viewModel;
        
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = _viewModel = viewModel;
            InitializeComponent();
        }
    }
}
