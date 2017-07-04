using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PholioVisualisation.Export.File
{
    public interface IFileBuilder
    {
        void AddContent(byte[] content);
        byte[] GetFileContent();
    }

    public class FileBuilder : IFileBuilder
    {
        private List<byte[]> _contents = new List<byte[]>();

        public void AddContent(byte[] content)
        {
            _contents.Add(content);
        }

        public byte[] GetFileContent()
        {
            var output = new byte[_contents.Sum(arr => arr.Length)];
            int writeIdx = 0;
            foreach (var byteArr in _contents)
            {
                byteArr.CopyTo(output, writeIdx);
                writeIdx += byteArr.Length;
            }
            return output;
        }
    }

    public class DummyFileBuilder : IFileBuilder
    {
        public void AddContent(byte[] content)
        {
        }

        public byte[] GetFileContent()
        {
            return new byte[] {};
        }
    }
}
