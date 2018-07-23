using System.IO;
using System.Web.Mvc;

namespace IndicatorsUI.MainUI.Helpers
{
    public class ResponseHelper
    {
        /// <summary>
        /// Used to return a data response with specific content type
        /// </summary>
        public static FileStreamResult GetFileStreamResult(byte[] data, string contentType)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, contentType);
        }

    }
}