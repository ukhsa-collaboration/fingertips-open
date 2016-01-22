using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace PholioVisualisation.SearchIndexing
{
    public abstract class IndexBuilder
    {
        private string directoryPath;

        protected IndexBuilder(string directoryPath)
        {
            this.directoryPath = directoryPath;
        }

        public abstract void BuildIndexes();

        /// <summary>
        ///     Common English short words are included, e.g. the, in, on
        /// </summary>
        /// <returns></returns>
        protected IndexWriter GetWriterThatIncludesStopWords(string type)
        {
            return new IndexWriter(
                GetIndexDirectory(type),
                new StandardAnalyzer(Version.LUCENE_29, new List<string>()),
                true,
                new IndexWriter.MaxFieldLength(25000));
        }

        protected Directory GetIndexDirectory(string type)
        {
            string path = Path.Combine(directoryPath, type);
            return FSDirectory.Open(new DirectoryInfo(path));
        }
    }
}