using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Database.Entities;

[Table("States")]
[Index(nameof(CreatedDate), IsDescending = [true])]
public class StateEntity
{
    public int Id { get; set; }
    public decimal Temperature { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}