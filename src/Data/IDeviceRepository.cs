using DeviceMonitoringApp.Models;

namespace DeviceMonitoringApp.Data;

public interface IDeviceRepository
{
    Task<List<Device>> GetAllDeviceAsync();
    Task SaveMeasurementAsync(Measurement measurement);
    Task<List<Measurement>> GetAllMeasurementAsync(Guid deviceId);
    Task ClearHistory();
    Task SaveDeviceAsync(Device device);
}