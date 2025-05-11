using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Data.Entities;

[DebuggerDisplay("{ToString()}")]
[Table("Temperatures")]
[Index(nameof(DeviceId), nameof(CreatedDate), IsDescending = [false, true])]
public class TemperatureEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    public string DeviceId { get; set; }
    
    public double State { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
    
    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(DeviceId)}: {DeviceId}, {nameof(State)}: {State}, {nameof(CreatedDate)}: {CreatedDate}";
    }
}