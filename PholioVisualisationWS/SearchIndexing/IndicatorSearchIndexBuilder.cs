using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchIndexing
{
    public class IndicatorSearchIndexBuilder : IndexBuilder
    {
        public const string DirectoryIndicators = "indicators";

        private IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        public IndicatorSearchIndexBuilder(string directoryPath)
            : base(directoryPath)
        {
        }

        public override void BuildIndexes()
        {
            IndexWriter writer = GetWriter(DirectoryIndicators);
            writer.DeleteAll();

            var properties = groupDataReader
                .GetIndicatorMetadataTextProperties()
                .Where(p => p.IsSearchable);

            IEnumerable<IndicatorMetadata> indicatorMetadataList = GetIndicatorMetadataList(properties);

            foreach (IndicatorMetadata indicatorMetadata in indicatorMetadataList)
            {
                IndexIndicator(indicatorMetadata, properties, writer);
            }

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }

        private static void IndexIndicator(IndicatorMetadata indicatorMetadata,
            IEnumerable<IndicatorMetadataTextProperty> properties, IndexWriter writer)
        {
            Document doc = new Document();
            doc.Add(new Field("id", indicatorMetadata.IndicatorId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            var text = indicatorMetadata.Descriptive;

            StringBuilder sb = new StringBuilder();
            foreach (var indicatorMetadataTextProperty in properties)
            {
                var key = indicatorMetadataTextProperty.ColumnName;

                if (text.ContainsKey(key))
                {
                    sb.Append(text[key]);
                    sb.Append(" ");
                }
            }

            doc.Add(new Field("IndicatorText",
                  sb.ToString().ToLower(), Field.Store.NO,
                  Field.Index.ANALYZED));

            writer.AddDocument(doc);
        }

        private IEnumerable<IndicatorMetadata> GetIndicatorMetadataList(IEnumerable<IndicatorMetadataTextProperty> properties)
        {
            var groupIds = ReaderFactory.GetProfileReader().GetGroupIdsFromAllProfiles();
            var groupings = groupDataReader.GetGroupingsByGroupIds(groupIds);
            return groupDataReader.GetIndicatorMetadata(groupings.ToList(), properties.ToList());
        }

        private IndexWriter GetWriter(string type)
        {
            return new IndexWriter(
                GetIndexDirectory(type),
                new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29),
                true,
                new IndexWriter.MaxFieldLength(25000));
        }
    }
}