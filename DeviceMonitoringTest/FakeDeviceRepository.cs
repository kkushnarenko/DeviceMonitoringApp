using DeviceMonitoringApp.Data;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringTest;


public class FakeDeviceRepository : IDeviceRepository 
{
    public Task<List<Device>> GetAllDeviceAsync() 
        => Task.FromResult(new List<Device>());
  
    public List<Measurement> SavedMeasurement { get; } = new();
    public List<Device> SavedDevice { get; } = new();

    public List<double?> LastValueDevice { get; } = new();
    
    public Task SaveMeasurementAsync(Measurement measurement)
    {
        lock (SavedMeasurement)
            SavedMeasurement.Add(measurement);
        return Task.CompletedTask;
    }

    public Task<List<Measurement>> GetAllMeasurementAsync(Guid deviceId)
        => Task.FromResult(new List<Measurement>());

    public Task ClearHistory()
    {
        SavedMeasurement.Clear();
        return Task.CompletedTask;
    }

    public Task SaveDeviceAsync(Device device)
    {
        lock (SavedDevice)
        {
            LastValueDevice.Add(device.LastValue);
            SavedDevice.Add(new Device
            {
                Id = device.Id,
                LastValue = device.LastValue,
                Status = device.Status,
            });
        }

        return Task.CompletedTask;
    }
}