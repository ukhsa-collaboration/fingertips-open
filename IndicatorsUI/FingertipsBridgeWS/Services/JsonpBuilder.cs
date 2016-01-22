using System;
using System.Collections.Generic;
using System.Text;

namespace FingertipsBridgeWS.Services
{
    public class JsonpBuilder
    {
        public byte[] Jsonp { get; set; }

        public JsonpBuilder(byte[] json, string jsonpCallback)
        {
            List<byte> list = new List<byte>();
            list.AddRange(Encoding.ASCII.GetBytes(jsonpCallback));
            list.Add((byte)'(');
            list.AddRange(json);
            list.Add((byte)')');

            Jsonp = list.ToArray();
        }
    }
}