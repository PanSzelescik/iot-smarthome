using System.ComponentModel.DataAnnotations.Schema;
using IotSmartHome.Database.Enums;

namespace IotSmartHome.Database.Entities;

[Table("Devices")]
public class DeviceEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DeviceType Type { get; set; }
    public int UserId { get; set; }
    public virtual UserEntity User { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    
    public List<TemperatureEntity> Temperatures { get; set; } = [];
    public List<StateEntity> States { get; set; } = [];
}