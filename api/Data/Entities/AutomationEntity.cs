using System.ComponentModel.DataAnnotations.Schema;
using IotSmartHome.Data.Enums;

namespace IotSmartHome.Data.Entities;

[Table("Automations")]
public class AutomationEntity
{
    public int Id { get; set; }
    
    public int UserThermometerId { get; set; }
    public virtual UserThermometerEntity UserThermometer { get; set; }
    
    public double WhenState { get; set; }
    
    public ConditionEnum WhenCondition { get; set; }
    
    public int ThenSwitchId { get; set; }
    public virtual UserSwitchEntity ThenSwitch { get; set; }
    
    public bool ThenState { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    public override string ToString()
    {
        return $"AutomationEntity {{ Id = {Id}, UserThermometerId = {UserThermometerId}, WhenState = {WhenState}, WhenCondition = {WhenCondition}, ThenSwitchId = {ThenSwitchId}, ThenState = {ThenState}, CreatedDate = {CreatedDate} }}";
    }
}