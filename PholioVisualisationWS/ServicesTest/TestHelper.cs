using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Debug.WriteLine(System.Text.Encoding.UTF8.GetString(data));
        }
    }
}
