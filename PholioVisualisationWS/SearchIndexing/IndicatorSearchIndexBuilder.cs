using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
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

        private IGroupDataReader _groupDataReader = ReaderFactory.GetGroupDataReader();

        private IEnumerable<IndicatorMetadataTextProperty> properties;

        public IndicatorSearchIndexBuilder(string directoryPath)
            : base(directoryPath)
        {
        }

        public override void BuildIndexes()
        {
            IndexWriter writer = GetWriter(DirectoryIndicators);
            writer.DeleteAll();

            properties = _groupDataReader
                .GetIndicatorMetadataTextProperties();

            IEnumerable<IndicatorMetadata> indicatorMetadataList = GetIndicatorMetadataList();

            foreach (IndicatorMetadata indicatorMetadata in indicatorMetadataList)
            {
                IndexIndicator(indicatorMetadata, writer);
            }

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }

        private void IndexIndicator(IndicatorMetadata indicatorMetadata, IndexWriter writer)
        {
            Document doc = new Document();

            IndexIndicatorId(indicatorMetadata, doc);
            IndexTextMetadata(indicatorMetadata, doc);

            writer.AddDocument(doc);
        }

        private void IndexIndicatorId(IndicatorMetadata indicatorMetadata, Document doc)
        {
            doc.Add(new Field("id", indicatorMetadata.IndicatorId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        private void IndexTextMetadata(IndicatorMetadata indicatorMetadata, Document doc)
        {
            var text = indicatorMetadata.Descriptive;

            foreach (var indicatorMetadataTextProperty in properties)
            {
                var key = indicatorMetadataTextProperty.ColumnName;
                var indexText = text.ContainsKey(key) ? text[key] : string.Empty;
                doc.Add(new Field(key, indexText.ToLower(), Field.Store.NO, Field.Index.ANALYZED));
            }
        }

        private IEnumerable<IndicatorMetadata> GetIndicatorMetadataList()
        {
            var groupIds = ReaderFactory.GetProfileReader().GetGroupIdsFromAllProfiles();
            var groupings = _groupDataReader.GetGroupingsByGroupIds(groupIds);

            IList<GroupingMetadata> groupingMetadata = _groupDataReader.GetGroupingMetadataList(groupIds);
            int profileId = groupingMetadata[0].ProfileId;

            return _groupDataReader.GetIndicatorMetadata(groupings.ToList(), properties.ToList(), profileId);
        }

        private IndexWriter GetWriter(string type)
        {
            var analyzer = new SynonymAnalyzer();

            return new IndexWriter(
                GetIndexDirectory(type), analyzer,
                true,
                new IndexWriter.MaxFieldLength(25000));
        }
    }
}