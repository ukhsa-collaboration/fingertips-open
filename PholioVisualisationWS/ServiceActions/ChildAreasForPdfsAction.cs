using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.ServiceActions
{
    public class ChildAreasForPdfsAction
    {
        public ChildAreasForPdfs GetResponse(int profileId, string parentAreaCode, int childAreaTypeId)
        {
            ParameterCheck.GreaterThanZero("Profile ID", profileId);
            ParameterCheck.GreaterThanZero("Child area type ID", childAreaTypeId);
            ParameterCheck.ValidAreaCode(parentAreaCode);

            var areasReader = ReaderFactory.GetAreasReader();
            var parentArea = areasReader.GetAreaFromCode(parentAreaCode);
            var ignoredAreasFilter = IgnoredAreasFilterFactory.New(profileId);

            var parentAreaWithChildren = new ParentChildAreaRelationshipBuilder(
                ignoredAreasFilter, new AreaListProvider(areasReader))
                     .GetParentAreaWithChildAreas(parentArea, childAreaTypeId, false/*retrieve ignored areas*/);

            // Benchmark hard coded for now
            var benchmarkArea = areasReader.GetAreaFromCode(AreaCodes.England);
            return new ChildAreasForPdfs(parentAreaWithChildren, benchmarkArea);
        }

    }
}
