using IotSmartHome.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, IdentityRole<int>, int>(options)
{
    public DbSet<DeviceEntity> Devices { get; set; }
    public DbSet<StateEntity> States { get; set; }
    public DbSet<TemperatureEntity> Temperature { get; set; }
}