using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.PdfData
{
    public abstract class DomainDataBuilder
    {
        protected IProfileReader profileReader = ReaderFactory.GetProfileReader();
        protected IGroupDataReader groupDataReader = ReaderFactory.GetGroupDataReader();

        protected List<DomainData> BuildDomainDataForProfile(int profileId, int childAreaTypeId,
            IList<string> benchmarkAreaCodes)
        {
            var dataList = new List<DomainData>();

            var groupIds = new GroupIdProvider(profileReader).GetGroupIds(profileId);
            var groupMetadataList = groupDataReader.GetGroupMetadataList(groupIds);
            var groupDataRepository = new GroupDataAtDataPointRepository
            {
                AssignAreas = false,
                AssignChildAreaData = false
            };

            var benchmarkAreas = ReaderFactory.GetAreasReader().GetAreasFromCodes(benchmarkAreaCodes);

            foreach (var groupMetadata in groupMetadataList)
            {
                var tableData = NewDomainData();
                var groupId = groupMetadata.Id;
                tableData.DomainTitle = groupMetadata.Name;
                tableData.GroupId = groupId;

                var groupData = groupDataRepository.GetGroupData(AreaCodes.England,
                    childAreaTypeId, profileId, groupId);
                var groupRoots = groupData.GroupRoots;

                foreach (var groupRoot in groupRoots)
                {
                    var metadata = groupData.GetIndicatorMetadataById(groupRoot.IndicatorId);
                    AddIndicatorData(groupRoot, metadata, benchmarkAreas);
                }

                dataList.Add(tableData);
            }

            return dataList;
        }

        protected abstract DomainData NewDomainData();
        protected abstract void AddIndicatorData(GroupRoot groupRoot, IndicatorMetadata metadata, IList<Area> benchmarkAreas);
    }
}
