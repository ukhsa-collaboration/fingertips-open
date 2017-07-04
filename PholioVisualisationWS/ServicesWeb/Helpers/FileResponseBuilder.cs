using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ServicesWeb.Helpers
{
    public class FileResponseBuilder
    {
        private HttpResponseMessage _message;

        public HttpResponseMessage Message {
            get { return _message;}
        }

        public FileResponseBuilder()
        {
            _message = new HttpResponseMessage(HttpStatusCode.OK);
        }

        public void SetFileContent(byte[] content)
        {
            _message.Content = new ByteArrayContent(content);
            _message.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
        }

        public void SetFilename(string file_name)
        {
            _message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = file_name
            };
        }

        public static HttpResponseMessage NewMessage(byte[] bytes, string filename)
        {
            var responseBuilder = new FileResponseBuilder();
            responseBuilder.SetFileContent(bytes);
            responseBuilder.SetFilename(filename);
            return responseBuilder.Message;
        }
    }
}