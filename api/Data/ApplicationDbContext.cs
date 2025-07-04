﻿using IotSmartHome.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserEntity, IdentityRole<int>, int>(options)
{
    public DbSet<TemperatureEntity> Temperatures { get; set; }
    public DbSet<UserThermometerEntity> UserThermometers { get; set; }
    
    public DbSet<SwitchEntity> Switches { get; set; }
    public DbSet<UserSwitchEntity> UserSwitches { get; set; }
    
    public DbSet<AutomationEntity> Automations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole<int>>().HasData(new IdentityRole<int>
        {
            Id = AppConsts.RoleAdminId,
            Name = AppConsts.RoleAdmin,
            NormalizedName = AppConsts.RoleAdmin.Normalize().ToUpperInvariant(),
        });
    }
}