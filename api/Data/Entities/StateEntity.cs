using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data.Entities;

[Table("States")]
[Index(nameof(CreatedDate), IsDescending = [true])]
public class StateEntity
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public virtual DeviceEntity Device { get; set; }
    public bool State { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}