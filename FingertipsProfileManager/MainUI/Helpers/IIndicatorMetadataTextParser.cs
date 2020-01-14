using System.Collections.Generic;
using Fpm.ProfileData;

namespace Fpm.MainUI.Helpers
{
    public interface IIndicatorMetadataTextParser
    {
        IList<IndicatorMetadataTextItem> Parse(string concatenatedMetadataProperties);
    }
}