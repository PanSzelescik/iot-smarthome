using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data.Entities;

[Table("UserSwitch")]
[Index(nameof(UserId), nameof(DeviceId), IsUnique = true)]
public class UserSwitchEntity
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual UserEntity User { get; set; }
    
    public string DeviceId { get; set; }
    
    public string FriendlyName { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
}