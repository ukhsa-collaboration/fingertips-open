using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class PracticeValueCollection
    {
        public PracticeAxis PracticeAxis1;
        public PracticeAxis PracticeAxis2;

        public double PracticeValue1 = ValueData.NullValue;
        public double PracticeValue2 = ValueData.NullValue;

        public IEnumerable<string> ChildAreaCodes;

        public bool IsPracticeData
        {
            get { return PracticeValue1 != ValueData.NullValue && PracticeValue2 != ValueData.NullValue; }
        }

        public bool IsData
        {
            get
            {
                return PracticeAxis1 != null && PracticeAxis2 != null &&
                    PracticeAxis1.IndicatorData.Count > 0 &&
                    PracticeAxis2.IndicatorData.Count > 0;
            }
        }
    }
}
