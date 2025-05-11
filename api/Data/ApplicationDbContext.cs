using IotSmartHome.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, IdentityRole<int>, int>(options)
{
    public DbSet<TemperatureEntity> Temperatures { get; set; }
}