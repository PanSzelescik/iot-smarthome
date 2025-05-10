using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data.Entities;

[Table("Temperatures")]
[Index(nameof(CreatedDate), IsDescending = [true])]
public class TemperatureEntity
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public virtual DeviceEntity Device { get; set; }
    public decimal Temperature { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}