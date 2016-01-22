/* 
 * Created by: Daniel Flint    
 * Date: 19/07/2011 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class IndicatorIndexBuilder : ExcelFileBuilder
    {
        public string FilePath { get; set; }

        public override byte[] CreateFile()
        {
            IndicatorIndexWriter writer = new IndicatorIndexWriter();
            ProfilesReader reader = new ProfilesReader();
            GroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();
            IndicatorMetadataRepository repository = IndicatorMetadataRepository.Instance;

            IList<int> ids = reader.GetAllProfileIds();

            var allGroupIds = reader.GetAllExportGroups().GroupIds;
            var areaTypeIds = groupDataReader.GetDistinctGroupingAreaTypeIds(allGroupIds);

           /* IEnumerable<Profile> profiles = ids
                .Select(id => new ProfileBuilder().BuildByParentAreaCode(id, "Q35", ReaderFactory.GetAreasReader(), areaTypeIds))
                .Where(x => x.IsDefinedProfile);

            foreach (Profile profile in profiles)
            {
                var groupIds = profile.GroupIds;
                bool areDomains = groupIds.Count > 0;

                writer.InitNewProfile(profile, areDomains);

                List<int> indicatorIds = new List<int>();
                var groupMetadataList = groupDataReader.GetGroupMetadata(groupIds);

                foreach (var groupMetadata in groupMetadataList)
                {
                    string domainName = null;
                    if (areDomains)
                    {
                        domainName = groupMetadata.Name;
                    }

                    IList<Grouping> groupings = groupDataReader.GetGroupData(groupMetadata.Id);
                    foreach (Grouping grouping in groupings)
                    {
                        if (indicatorIds.Contains(grouping.IndicatorId) == false)
                        {
                            indicatorIds.Add(grouping.IndicatorId);

                            IndicatorMetadata metadata = repository.GetIndicatorMetadata(grouping);

                            SpecifiedTimePeriodFormatter formatter = new SpecifiedTimePeriodFormatter
                            {
                                TimePeriod = TimePeriod.GetDataPoint(grouping)
                            };

                            writer.AddIndicator(metadata, formatter.Format(metadata), domainName);
                        }
                    }
                }
            } */

            return writer.Write(FilePath);
        }


    }
}
