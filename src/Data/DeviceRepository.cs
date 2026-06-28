using DeviceMonitoringApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DeviceMonitoringApp.Data;

public class DeviceRepository : IDeviceRepository
{
    private SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<List<Device>> GetAllDeviceAsync()
    {
        using var db = new AppDbContext();
        return await db.Devices.ToListAsync();
    }

    public async Task SaveMeasurementAsync(Measurement measurement)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var db = new AppDbContext();
            await db.AddAsync(measurement);
            await db.SaveChangesAsync();
        }
       finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Measurement>> GetAllMeasurementAsync(Guid deviceId)
    {
        using var db = new AppDbContext();
        var data = await db.Measurements
            .Where(u => u.DeviceId == deviceId)
            .OrderByDescending(u => u.Timestamp)
            .ToListAsync();
        return data;
    }

    public async Task ClearHistory()
    {
        await _semaphore.WaitAsync();
        try
        {
            using var db = new AppDbContext();
            db.Measurements.RemoveRange(db.Measurements);
            await db.SaveChangesAsync();
        }
       finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task SaveDeviceAsync(Device device)
    {
        await _semaphore.WaitAsync();
        try
        {
            using var db = new AppDbContext();
            db.Devices.Update(device);
            await db.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}