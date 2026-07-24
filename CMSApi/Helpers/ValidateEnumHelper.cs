using Core.Exceptions;

namespace CMSApi.Helpers
{
    public static class ValidateEnumHelper
    {
        public static void ValidateEnumValue<TEnum>(TEnum value, string enumName)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new CarEnumNotDefinedException(enumName);
        }
    }
}
