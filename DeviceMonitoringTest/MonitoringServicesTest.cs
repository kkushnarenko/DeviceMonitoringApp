using DeviceMonitoringApp.Services;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringTest;

[TestClass]
public sealed class MonitoringServicesTest
{
    [TestMethod]
    public async Task MonitoringServicesTestAsync_SuccessStartAndSaveMeasurement()
    {
        var fakeRepository = new FakeDeviceRepository();
        var services = new MonitoringService(fakeRepository);
        var testDevice = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый датчик",
            Status = DeviceStatus.Online,
            Type = DeviceType.TemperatureSensor,
            PollingInterval = 400
        };
        
        var device = new List<Device> { testDevice };
        
        await services.StartMonitoringAsync(device);
        await Task.Delay(2000);
        
        Assert.IsNotEmpty(fakeRepository.SavedMeasurement, "Фоновые потоки не сохранили ни одного измерения");
        Assert.AreEqual(testDevice.Id, fakeRepository.SavedMeasurement[0].DeviceId);

    }
    
    [TestMethod]
    public async Task MonitoringServicesTestAsync_UpdateStatusDevice()
    {
        var fakeRepository = new FakeDeviceRepository();
        var services = new MonitoringService(fakeRepository);
        var testDevice = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый датчик",
            Status = DeviceStatus.Offline,
            Type = DeviceType.TemperatureSensor,
            PollingInterval = 400
        };
        var device = new List<Device> { testDevice };
        await services.StartMonitoringAsync(device);
        await Task.Delay(50);
        Assert.AreEqual(DeviceStatus.Online, device[0].Status);
        services.StopMonitoring();
        await Task.Delay(50);
        Assert.AreEqual(DeviceStatus.Offline, device[0].Status);
    }
    
    [TestMethod]
    public async Task MonitoringServicesTestAsync_SaveErrorMessage()
    {
        var fakeRepository = new FakeDeviceRepository();
        var services = new MonitoringService(fakeRepository);
        var testDevice = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый датчик",
            Status = DeviceStatus.Offline,
            Type = DeviceType.TemperatureSensor,
            PollingInterval = 100
        };
        var device = new List<Device> { testDevice };
        
        await services.StartMonitoringAsync(device);
        await Task.Delay(1000);
        services.StopMonitoring();

        var errorLogs = fakeRepository.SavedMeasurement
            .Where(x => !x.IsSuccess).ToList();
        if (errorLogs.Count > 0)
        {
            errorLogs.ForEach(x => Assert.AreEqual(0, x.Value));
            foreach (var logs in errorLogs)
                Assert.IsNotEmpty(logs.ErrorMessage);
        }
        else
            Assert.Inconclusive("Выданы успешные логи, запустите тест еще раз");
    }

    [TestMethod]
    public async Task MonitoringServicesTestAsync_UpdateDeviceEvent()
    {
        var fakeRepository = new FakeDeviceRepository();
        var services = new MonitoringService(fakeRepository);
        var testDevice = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый датчик",
            Status = DeviceStatus.Offline,
            Type = DeviceType.TemperatureSensor,
            PollingInterval = 100
        };
        var device = new List<Device> { testDevice };
        int eventCallCount = 0;
        services.DeviceUpdated += (updatedDevice) => { eventCallCount++; };
        
        await services.StartMonitoringAsync(device);
        await Task.Delay(1000);
        services.StopMonitoring();
        await Task.Delay(50);
        
        Assert.IsTrue(eventCallCount > 0);
    }

    [TestMethod]
    public async Task MonitoringServicesTestAsync_UpdateDeviceLastData()
    {
        var fakeRepository = new FakeDeviceRepository();
        var services = new MonitoringService(fakeRepository);
        var testDevice = new Device
        {
            Id = Guid.NewGuid(),
            Name = "Тестовый датчик",
            Status = DeviceStatus.Offline,
            Type = DeviceType.TemperatureSensor,
            PollingInterval = 100
        };
        
        var device = new List<Device> { testDevice };
        
        await services.StartMonitoringAsync(device);
        await Task.Delay(2000);
        
        services.StopMonitoring();
        await Task.Delay(50);
        Assert.IsNotEmpty(fakeRepository.SavedDevice, "Фоновые потоки не сохранили ни одного измерения");

        var lastIndexData = fakeRepository.SavedDevice.Count - 1;
        var lastData = fakeRepository.SavedDevice[lastIndexData];
        
        Assert.AreEqual(testDevice.LastValue, lastData.LastValue);
        
    }
}