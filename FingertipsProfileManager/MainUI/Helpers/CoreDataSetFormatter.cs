namespace Fpm.MainUI.Helpers
{
    public class CoreDataSetFormatter
    {
        public static string Format(double? val)
        {
            return val.HasValue ?
                val.Round().ToString()
                : "-";
        }
    }
}