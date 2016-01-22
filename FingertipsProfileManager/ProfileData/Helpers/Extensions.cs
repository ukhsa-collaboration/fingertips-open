using System.Collections.Generic;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.ProfileData.Helpers
{
    public static class Extensions
    {

        public static void SetPropertyValue(this IndicatorMetadataTextValue obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName).SetValue(obj, value, null);
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }
            if (nextbatch.Count > 0)
                yield return nextbatch;
        }
        
    }
}
