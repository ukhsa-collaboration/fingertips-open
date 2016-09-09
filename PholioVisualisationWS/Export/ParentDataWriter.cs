using System.Collections.Generic;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public abstract class ParentDataWriter
    {
        public IAreasReader AreasReader;
        public IGroupDataReader GroupDataReader;
        public WorksheetInfo Worksheet;
        protected ProfileDataWriter ProfileDataWriter;

        protected ParentDataWriter(IAreasReader areasReader, IGroupDataReader groupDataReader, WorksheetInfo worksheetInfo, ProfileDataWriter profileDataWriter)
        {
            AreasReader = areasReader;
            GroupDataReader = groupDataReader;
            Worksheet = worksheetInfo;
            ProfileDataWriter = profileDataWriter;
        }

        public abstract IList<CoreDataSet> AddMultipleAreaData(RowLabels rowLabels, Grouping grouping, TimePeriod timePeriod,
            IndicatorMetadata metadata, Dictionary<string, Area> areaCodeToParentMap);

        public abstract IList<CategoryIdAndAreaCode> CategoryIdAndAreaCodes { get;  }
    }

    public static class ParentDataWriterFactory
    {
        public static ParentDataWriter New(IAreasReader areasReader, IGroupDataReader groupDataReader, WorksheetInfo worksheetInfo, 
            ProfileDataWriter profileDataWriter, IAreaType parentAreaType)
        {
            var categoryAreaType = parentAreaType as CategoryAreaType;
            return categoryAreaType != null
                ? (ParentDataWriter)new CategoryAreaDataWriter(areasReader, groupDataReader, worksheetInfo, profileDataWriter, categoryAreaType)
                : (ParentDataWriter)new SubnationalAreaDataWriter(areasReader, groupDataReader, worksheetInfo, profileDataWriter, parentAreaType);
        }
    }
}