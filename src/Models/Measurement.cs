using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceMonitoringApp.Models;

public class Measurement
{
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public double? Value { get; set; }
    public DateTime Timestamp { get; set; } =  DateTime.Now;
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}