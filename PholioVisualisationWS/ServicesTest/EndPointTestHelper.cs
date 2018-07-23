using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PholioVisualisation.ServicesTest
{
    public static class EndPointTestHelper
    {
        public static byte[] GetData(string path)
        {
            var url = TestHelper.BaseUrl + "api/" + path;
            Debug.WriteLine(url);
            return new WebClient().DownloadData(url);
        }
    }
}
