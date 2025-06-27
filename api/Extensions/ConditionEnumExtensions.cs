using IotSmartHome.Data.Enums;

namespace IotSmartHome.Extensions;

public static class ConditionEnumExtensions
{
    public static bool IsTrue(this ConditionEnum condition, double value, double compareValue)
    {
        return condition switch
        {
            ConditionEnum.Equal => value == compareValue,
            ConditionEnum.NotEqual => value != compareValue,
            ConditionEnum.GreaterThan => value > compareValue,
            ConditionEnum.GreaterThanOrEqual => value >= compareValue,
            ConditionEnum.LessThan => value < compareValue,
            ConditionEnum.LessThanOrEqual => value <= compareValue,
            _ => throw new ArgumentOutOfRangeException(nameof(condition), condition, null)
        };
    }
}