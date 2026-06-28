using DeviceMonitoringApp.Models;
namespace DeviceMonitoringApp.Services;

public interface IMonitoringService
{
    event Action<Device>? DeviceUpdated;
    
    Task StartMonitoringAsync(IEnumerable<Device> devices);
    void StopMonitoring();
}