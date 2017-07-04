using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData.Helpers
{
    public class IndicatorDisclosureControlMapper
    {
        private Dictionary<int, int> _mappings = new Dictionary<int, int>();

        public void Set(int indicatorId, int disclosureControlId)
        {
            if (!_mappings.ContainsKey(indicatorId))
            {
                _mappings.Add(indicatorId, disclosureControlId);
            }
        }

        public int? GetDisclosureControlId(int indicatorId)
        {
            if (_mappings.ContainsKey(indicatorId))
            {
                return _mappings[indicatorId];
            }

            return null;
        }
    }
}
