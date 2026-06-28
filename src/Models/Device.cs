using DeviceMonitoringApp.ViewModels;

namespace DeviceMonitoringApp.Models;


public class Device : ViewModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }  = string.Empty;
    public DeviceType Type { get; set; }
    private DeviceStatus _status = DeviceStatus.Offline;

    public DeviceStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }
    public int PollingInterval { get; set; }
    
    private double? _lastValue;
    public double? LastValue 
    { 
        get => _lastValue;
        set
        {
            _lastValue = value;
            OnPropertyChanged();
        }
    }
    private DateTime? _lastUpdateTime;

    public DateTime? LastUpdateTime
    {
        get => _lastUpdateTime;
        set
        {
            _lastUpdateTime = value;
            OnPropertyChanged();
        }
    }
}