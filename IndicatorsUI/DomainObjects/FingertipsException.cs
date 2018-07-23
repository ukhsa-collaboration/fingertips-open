using System;

namespace IndicatorsUI.DomainObjects
{
    public class FingertipsException : Exception
    {
        public FingertipsException(string message)
            : base(message)
        {
        }

        public FingertipsException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
