namespace ServicesWeb.Common
{
    public class ServiceHelper
    {
        public static bool ParseYesOrNo(string yesOrNoString, bool defaultBool)
        {
            if (string.IsNullOrWhiteSpace(yesOrNoString))
            {
                return defaultBool;
            }

            yesOrNoString = yesOrNoString.ToLower();

            if (yesOrNoString.Equals("yes"))
            {
                return true;
            }

            if (yesOrNoString.Equals("no"))
            {
                return false;
            }

            return defaultBool;
        } 
    }
}