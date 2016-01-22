using System;

namespace Fpm.ProfileData
{
    public class FpmException : Exception
    {
        public FpmException(string message)
            : base(message)
        {
        }

        public FpmException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}