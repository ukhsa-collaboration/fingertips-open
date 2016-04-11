using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData.Helpers
{
    public static class Extensions
    {

        public static void SetPropertyValue(this IndicatorMetadataTextValue obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        }
    }
}
