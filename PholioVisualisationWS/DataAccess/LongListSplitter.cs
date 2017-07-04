using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public class LongListSplitter<T>
    {
        public const int NumberOfCodesToTakeAtOnce = 300;
        private readonly IList<T> _areaCodes;
        private IEnumerable<T> _nextAreaCodes;
        private int _numberTaken;

        public LongListSplitter(IList<T> areaCodes)
        {
            _areaCodes = areaCodes;
        }

        public bool AnyLeft()
        {
            return _numberTaken < _areaCodes.Count;
        }

        public IEnumerable<T> NextCodes()
        {
            _nextAreaCodes = _areaCodes.Skip(_numberTaken).Take(NumberOfCodesToTakeAtOnce);
            _numberTaken += NumberOfCodesToTakeAtOnce;
            return _nextAreaCodes;
        }
    }
}