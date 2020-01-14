using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace PholioVisualisation.ServicesTest
{
    public class TestHelper
    {
        public static string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["BaseUrl"]; }
        }

        public static void IsData(byte[] data)
        {
            Assert.AreNotEqual(0, data.Length);

            Debug.WriteLine(GetDataString(data));
        }

        public static void AssertDataContainsString(byte[] data, string s)
        {
            Assert.IsTrue(GetDataString(data).Contains(s));
        }

        public static byte[] GetData(string path)
        {
            var url = BaseUrl + "api/" + path;
            Debug.WriteLine(url);
            return new WebClient().DownloadData(url);
        }

        public static string GetDataString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static int GetRandomInt()
        {
            return BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
        }
    }
}
