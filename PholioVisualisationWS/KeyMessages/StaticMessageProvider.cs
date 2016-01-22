using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.KeyMessages
{
    public class StaticMessageProvider
    {
        private IList<KeyMessageOverride> messages;

        public StaticMessageProvider(IList<KeyMessageOverride> messages)
        {
            this.messages = messages;
        }

        public string GetMessage(int keyMessageId)
        {
            KeyMessageOverride message = messages.FirstOrDefault(x => x.MessageId == keyMessageId);
            if (message != null)
            {
                return message.MessageText;
            }

            return null;
        }
    }
}