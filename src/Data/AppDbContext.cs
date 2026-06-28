using Microsoft.EntityFrameworkCore;
using DeviceMonitoringApp.Models;

namespace DeviceMonitoringApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<Device> Devices { get; set; } = null;
    public DbSet<Measurement> Measurements { get; set; } = null;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=monitoring.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Device>().HasKey(d => d.Id);
        modelBuilder.Entity<Measurement>().HasKey(m => m.Id);
    }
  
}