using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DeviceMonitoringApp.Data;
using DeviceMonitoringApp.Services;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ObservableCollection<Device> Device { get; } = new();
    public ObservableCollection<Measurement> Measurements { get; } = new();

    private Device? _selectedDevice;

    public Device? SelectedDevice
    {
        get { return _selectedDevice; }
        set
        {
            _selectedDevice = value;
            OnPropertyChanged();

            _ = UpdateMeasurementsForSelectedDevice();
        }
    }
    
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ClearStory { get; }

    private string _systemStatus = "Готов к работе";

    public string SystemStatusText
    {
        get { return _systemStatus; }
        set
        {
            _systemStatus = value;
            OnPropertyChanged();
        }
    }

    private IMonitoringService _monitoringService { get; }
    private IDeviceRepository _deviceRepository { get; }

    public MainViewModel(IMonitoringService monitoringService, IDeviceRepository repository)
    {
        _monitoringService = monitoringService;
        _deviceRepository = repository;

        StartCommand = new RelayCommand(async (obj) => await StartMonitoringAsync());
        StopCommand = new RelayCommand((obj) => StopMonitoring());
        ClearStory = new RelayCommand(async (obj)=> await ClearStoryAsync());

        _monitoringService.DeviceUpdated += LoadMeasures;
        LoadDevicesAsync();
    }

    private async Task ClearStoryAsync()
    {
        await _deviceRepository.ClearHistory();
        Measurements.Clear();
    }

    private async Task StartMonitoringAsync()
    {
        if(SystemStatusText == "Мониторинг запущен") 
            return;
        SystemStatusText = "Мониторинг запущен";
        await _monitoringService.StartMonitoringAsync(Device);
    }

    private void StopMonitoring()
    {
        _monitoringService.StopMonitoring();
        SystemStatusText = "Мониторинг остановлен";
    }

    private async void LoadDevicesAsync()
    {
        try
        {
            var devices = await _deviceRepository.GetAllDeviceAsync();
            Device.Clear();
            foreach (var device in devices)
                Device.Add(device);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка загрузки устройств: {e.Message}");
            SystemStatusText = "Ошибка";
        }
    }

    private void LoadMeasures(Device device)
    {
        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var index = Device.IndexOf(device);
            if (index >= 0)
                Device[index] = device;

            if (SelectedDevice != null && SelectedDevice.Id == device.Id)
                await UpdateMeasurementsForSelectedDevice();
            
        });
    }

    private async Task UpdateMeasurementsForSelectedDevice()
    {
        if (SelectedDevice == null)
        {
            Measurements.Clear();
            return;
        }

        try
        {
            var latestMeasurement = await _deviceRepository.GetAllMeasurementAsync(SelectedDevice.Id);

            Measurements.Clear();
            foreach (var measurement in latestMeasurement)
                Measurements.Add(measurement);
        }
        catch (Exception e)
        {
            SystemStatusText = "Ошибка выгрузки истории";
            MessageBox.Show(e.Message);
        }
    }

}   