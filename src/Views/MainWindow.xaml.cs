using System.Windows;
using DeviceMonitoringApp.ViewModels;

namespace DeviceMonitoringApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var repo = new Data.DeviceRepository();
        var service = new Services.MonitoringService(repo);

        DataContext = new MainViewModel(service, repo);
    }
}