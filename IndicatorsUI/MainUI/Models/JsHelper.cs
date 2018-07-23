
namespace IndicatorsUI.MainUI.Models
{
    public static class JsHelper
    {
        public static string GetJsBool(bool? b)
        {
            return (b ?? false) ? "true" : "false";
        }

        public static string GetIncludePath(string file, string path)
        {
            return file.StartsWith("/")
                       ? "http:" + file
                       : path + file;
        }
    }
}