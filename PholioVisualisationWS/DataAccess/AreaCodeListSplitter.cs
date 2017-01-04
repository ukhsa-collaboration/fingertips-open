using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public class AreaCodeListSplitter
    {
        public const int NumberOfCodesToTakeAtOnce = 1000;
        private readonly IList<string> _areaCodes;
        private IEnumerable<string> _nextAreaCodes;
        private int _numberTaken;

        public AreaCodeListSplitter(IList<string> areaCodes)
        {
            _areaCodes = areaCodes;
        }

        public bool AnyLeft()
        {
            return _numberTaken < _areaCodes.Count;
        }

        public IEnumerable<string> NextCodes()
        {
            _nextAreaCodes = _areaCodes.Skip(_numberTaken).Take(NumberOfCodesToTakeAtOnce);
            _numberTaken += NumberOfCodesToTakeAtOnce;
            return _nextAreaCodes;
        }
    }
}