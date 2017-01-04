using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.SearchIndexing
{
    public class GeographicalSearchIndexBuilder : IndexBuilder
    {
        public const string DirectoryPlacePlacecodes = "placePostcodes";

        private readonly IAreasReader areasReader = ReaderFactory.GetAreasReader();

        private List<int> parentAreaTypeIds;
        private Dictionary<int, int> parentAreaTypeIdHash;

        private Dictionary<string, string> parentAreaCodeToName = new Dictionary<string, string>();

        public GeographicalSearchIndexBuilder(string directoryPath)
            : base(directoryPath)
        {
        }

        public override void BuildIndexes()
        {
            IndexWriter writer = GetWriterThatIncludesStopWords(DirectoryPlacePlacecodes);
            writer.DeleteAll();

            InitSearchableAreaTypes();
            InitAreaLookUps();

            IndexPlaceNames(writer);
            IndexPostcodes(writer);
            IndexParentAreas(writer);

            writer.Optimize();
            writer.Commit();
            writer.Close();
        }

        private void InitSearchableAreaTypes()
        {
            parentAreaTypeIds = areasReader
                 .GetAllAreaTypes()
                 .Where(x => x.IsSearchable)
                 .Select(x => x.Id)
                 .ToList();

            parentAreaTypeIdHash = parentAreaTypeIds.ToDictionary(x => x, x => x);
        }

        private void InitAreaLookUps()
        {
            foreach (var parentAreaTypeId in parentAreaTypeIds)
            {
                var areas = areasReader.GetAreasByAreaTypeId(parentAreaTypeId);
                foreach (var area in areas)
                {
                    var areaCode = area.Code;
                    if (parentAreaCodeToName.ContainsKey(areaCode) == false)
                    {
                        parentAreaCodeToName.Add(areaCode, area.Name);
                    }
                }
            }
        }

        private void IndexParentAreas(IndexWriter writer)
        {
            foreach (var areaTypeId in parentAreaTypeIds)
            {
                IndexParentAreas(writer, areasReader.GetAreasByAreaTypeId(areaTypeId), areaTypeId);
            }
        }

        private void IndexParentAreas(IndexWriter writer, IEnumerable<IArea> areas, int areaTypeId)
        {
            foreach (var county in areas)
            {
                IndexParentArea(county, writer, areaTypeId);
            }
        }

        private void IndexPostcodes(IndexWriter writer)
        {
            var postcodeProvider = new PostcodeProvider(areasReader);
            while (postcodeProvider.AreMorePostcodes)
            {
                var postcodes = postcodeProvider.GetNextPostcodeBatch();
                foreach (PostcodeParentAreas postcode in postcodes)
                {
                    IndexPostcode(postcode, writer);
                }
            }
        }

        private void IndexPlaceNames(IndexWriter writer)
        {
            var placeNames = areasReader.GetAllPlaceNames();
            foreach (var placeName in placeNames)
            {
                IndexPlaceName(placeName, writer);
            }
        }

        private void IndexParentArea(IArea area, IndexWriter writer, int childAreaTypeId)
        {
            Document doc = new Document();
            AddAnalysedField(doc, FieldNames.PlaceName, area.Name.ToLower());
            AddNameFormatted(area.Name, doc);
            AddCounty(string.Empty, doc);
            AddParentArea(area.Code, area.Name, doc, childAreaTypeId);

            // Null value for all other possible parent area types
            foreach (var areaTypeId in parentAreaTypeIds)
            {
                if (areaTypeId != childAreaTypeId)
                {
                    AddNullParentAreaCode(doc, areaTypeId);
                }
            }

            AddPlaceTypeWeighting(3/*After cities but before towns*/, doc);
            writer.AddDocument(doc);
        }

        private void IndexPostcode(PostcodeParentAreas postcode, IndexWriter writer)
        {
            Document doc = new Document();
            AddAnalysedField(doc, FieldNames.Postcode, postcode.Postcode.ToLower());
            AddCounty(GetCounty(postcode), doc);
            AddParentAreaCodes(postcode, doc);
            AddPlaceTypeWeighting(postcode.PlaceTypeWeighting, doc);
            AddEastingAndNorthing(doc, postcode.Easting, postcode.Northing);
            writer.AddDocument(doc);
        }

        private void IndexPlaceName(PlaceName placename, IndexWriter writer)
        {
            Document doc = new Document();
            AddAnalysedField(doc, FieldNames.PlaceName, placename.Name.ToLower());
            AddNameFormatted(placename.Name, doc);
            AddCounty(GetCounty(placename), doc);
            AddParentAreaCodes(placename.PostcodeParentAreas, doc);
            AddPlaceTypeWeighting(placename.PlaceTypeWeighting, doc);
            AddEastingAndNorthing(doc, placename.Easting, placename.Northing);
            writer.AddDocument(doc);
        }

        private static void AddEastingAndNorthing(Document doc, int easting, int northing)
        {
            doc.Add(GetIntField(FieldNames.Easting, easting, false));
            doc.Add(GetIntField(FieldNames.Northing, northing, false));
        }

        private static void AddNameFormatted(string name, Document doc)
        {
            doc.Add(new Field(FieldNames.NameFormatted, name, Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        private void AddParentAreaCodes(PostcodeParentAreas parentAreas, Document doc)
        {
            AddParentAreaMapping(doc, AreaTypeIds.GoRegion, parentAreas.AreaCode6);
            AddParentAreaMapping(doc, AreaTypeIds.CountyAndUnitaryAuthority, parentAreas.AreaCode102);
            AddParentAreaMapping(doc, AreaTypeIds.DistrictAndUnitaryAuthority, parentAreas.AreaCode101);
            AddParentAreaMapping(doc, AreaTypeIds.Ccg, parentAreas.AreaCode19);
            AddParentAreaMapping(doc, AreaTypeIds.Subregion, parentAreas.AreaCode46);
            AddParentAreaMapping(doc, AreaTypeIds.PheCentresFrom2013To2015, parentAreas.AreaCode103);
            AddParentAreaMapping(doc, AreaTypeIds.PheCentresFrom2015, parentAreas.AreaCode104);
        }

        private void AddParentAreaMapping(Document doc, int areaTypeId, string areaCode)
        {
            if (parentAreaTypeIdHash.ContainsKey(areaTypeId))
            {
                try
                {
                    AddParentArea(areaCode, parentAreaCodeToName[areaCode], doc, areaTypeId);
                }
                catch (KeyNotFoundException)
                {
                    ThrowInvalidParentCode(areaCode, areaTypeId);
                }
            }
        }

        private static void ThrowInvalidParentCode(string areaCode, int areaTypeId)
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine("Invalid parent area code: " + areaCode)
                .AppendLine("For area type: " + areaTypeId)
                .AppendLine()
                .AppendLine("Maybe invalid ParentLevelGeographyCode:")
                .AppendLine(
                    "SELECT DISTINCT ParentLevelGeographyCode FROM L_AreaMapping WHERE ParentLevelGeographyCode NOT IN(SELECT areacode FROM l_areas)")
                .AppendLine()
                .AppendLine("Or maybe invalid code in GIS_Postcodes");
            

            throw new FingertipsException(sb.ToString());
        }

        private void AddCounty(string county, Document doc)
        {
            doc.Add(new Field(FieldNames.County, county, Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        private void AddParentArea(string areaCode, string areaName, Document doc, int areaTypeId)
        {
            AddAnalysedField(doc, "Parent_Area_Code_" + areaTypeId, areaCode);
            AddAnalysedField(doc, "Parent_Area_Name_" + areaTypeId, areaName);
        }

        private void AddNullParentAreaCode(Document doc, int areaTypeId)
        {
            AddParentArea("x", "x", doc, areaTypeId);
        }

        private static void AddPlaceTypeWeighting(int weighting, Document doc)
        {
            doc.Add(GetIntField(FieldNames.PlaceTypeWeighting, weighting, true));
        }

        private static NumericField GetIntField(string fieldName, int val, bool isIndexed)
        {
            return new NumericField(fieldName, 1, Field.Store.YES, isIndexed).SetIntValue(val);
        }

        private static void AddAnalysedField(Document doc, string fieldName, string property)
        {
            doc.Add(new Field(fieldName, property, Field.Store.YES, Field.Index.ANALYZED));
        }

        private string GetCounty(PlaceName placeName)
        {
            return parentAreaCodeToName[placeName.PostcodeParentAreas.AreaCode102];
        }

        private string GetCounty(PostcodeParentAreas postcode)
        {
            return parentAreaCodeToName[postcode.AreaCode102];
        }
    }
}