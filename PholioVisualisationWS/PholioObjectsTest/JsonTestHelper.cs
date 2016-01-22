using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PholioObjectsTest
{
    public static class JsonTestHelper
    {
        public static void AssertPropertyIsSerialised(object o, string propertyName)
        {
            Assert.IsTrue(JsonConvert.SerializeObject(o).Contains(propertyName));
        }

        public static void AssertPropertyIsNotSerialised(object o, string propertyName)
        {
            Assert.IsFalse(JsonConvert.SerializeObject(o).Contains(propertyName));
        }
    }
}