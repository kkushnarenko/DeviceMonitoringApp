using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using DeviceMonitoringApp.Data;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        if (!db.Devices.Any())
        {
            var devices = new List<Device>
            {
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Датчик температуры",
                    Type = DeviceType.TemperatureSensor,
                    Status = DeviceStatus.Offline,
                    PollingInterval = 2000
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Датчик давления",
                    Type = DeviceType.PressureSensor,
                    Status = DeviceStatus.Offline,
                    PollingInterval = 3000
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Вольтметр",
                    Type = DeviceType.VoltageMeter,
                    Status = DeviceStatus.Offline,
                    PollingInterval = 1500
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    Name = "Амперметр",
                    Type = DeviceType.CurrentMeter,
                    Status = DeviceStatus.Offline,
                    PollingInterval = 4000
                },
            };
            db.Devices.AddRange(devices);
            db.SaveChanges();
        }
    }
}