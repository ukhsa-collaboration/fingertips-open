using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.KeyMessages;
using PholioVisualisation.PholioObjects;

namespace KeyMessagesTest
{
    [TestClass]
    public class StaticMessageProviderTest
    {
        [TestMethod]
        public void TestGetMessage()
        {
            var messages = new List<KeyMessageOverride> {
                new KeyMessageOverride {MessageId = 1, MessageText = "a"},
                new KeyMessageOverride {MessageId = 2, MessageText = "b"}
            };

            Assert.AreEqual("b", new StaticMessageProvider(messages).GetMessage(2));
        }

        [TestMethod]
        public void TestGetMessage_EmptyList()
        {
            Assert.IsNull(new StaticMessageProvider(new List<KeyMessageOverride>()).GetMessage(1));
        }
    }
}
