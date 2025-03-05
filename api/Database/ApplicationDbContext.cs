using IotSmartHome.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<StateEntity> States { get; set; }
}