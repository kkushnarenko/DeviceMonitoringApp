using System.Diagnostics;
using System.Windows;
using DeviceMonitoringApp.Data;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringApp.Services;

public class MonitoringService : IMonitoringService
{
    private readonly IDeviceRepository _deviceRepository;
    private CancellationTokenSource? _cts;
    
    public event Action<Device>? DeviceUpdated;
    
    public MonitoringService(IDeviceRepository deviceRepository) 
        => _deviceRepository = deviceRepository;
        
    public Task StartMonitoringAsync(IEnumerable<Device> devices)
    {
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        foreach (var device in devices)
            _ = Task.Run(async () => await PollDeviceInternalAsync(device, ct), ct);
        return Task.CompletedTask;
    }

    public void StopMonitoring() 
        => _cts?.Cancel();

    private async Task PollDeviceInternalAsync(Device? device, CancellationToken ct)
    {
        Random random = new Random();

        try
        {
            ArgumentNullException.ThrowIfNull(device);

            device.Status = DeviceStatus.Online;
            device.LastValue = null;
            device.LastUpdateTime = DateTime.Now;

            await _deviceRepository.SaveDeviceAsync(device);
            DeviceUpdated?.Invoke(device);

            while (!ct.IsCancellationRequested)
            {

                await Task.Delay(device.PollingInterval, ct);

                byte statusDevice = (byte)random.Next(1, 4);
                device.Status = (DeviceStatus)statusDevice;
                await _deviceRepository.SaveDeviceAsync(device);
                var isSuccess = statusDevice == 1;
                var dateUpdate = DateTime.Now;

                string errorMessage = string.Empty;
                switch (statusDevice)
                {
                    case 1:
                        errorMessage = string.Empty;
                        device.LastValue = Math.Round(random.NextDouble() * 20 + 15, 1);
                        break;
                    case 2:
                        errorMessage = "Ошибка! Устройство не в сети";
                        device.LastValue = null;
                        break;
                    case 3:
                        errorMessage = "Ошибка!";
                        device.LastValue = null;
                        break;
                }

                var measurement =
                    (new Measurement()
                        {
                            Id = Guid.NewGuid(),
                            DeviceId = device.Id,
                            Value = device.LastValue ?? 0,
                            Timestamp = dateUpdate,
                            IsSuccess = isSuccess,
                            ErrorMessage = errorMessage
                        }
                    );

                await _deviceRepository.SaveMeasurementAsync(measurement);
                await _deviceRepository.SaveDeviceAsync(device);
                DeviceUpdated?.Invoke(device);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (device != null)
            {
                device.Status = DeviceStatus.Offline;
                device.LastUpdateTime = DateTime.Now;
            
                await _deviceRepository.SaveDeviceAsync(device);
                DeviceUpdated?.Invoke(device);
            }
        }
    }
}