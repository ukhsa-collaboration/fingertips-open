using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataSorting;

namespace PholioVisualisation.DataConstruction
{
    public class AreaListProvider
    {
        public virtual List<IArea> Areas { get; private set; }

        private IAreasReader areasReader;

        /// <summary>
        /// Parameterless constructor to enable mocking
        /// </summary>
        public AreaListProvider() { }

        public AreaListProvider(IAreasReader areasReader)
        {
            this.areasReader = areasReader;
        }

        public void CreateAreaListFromAreaCodes(IEnumerable<string> areaCodes)
        {
            Areas = areaCodes.Select(areaCode => AreaFactory.NewArea(areasReader, areaCode)).ToList();
        }

        public void CreateAreaListFromAreaTypeId(int profileId, int areaTypeId)
        {
            if (areaTypeId > NeighbourAreaType.IdAddition) 
            {
                IList<Area> areas = new List<Area>
                    {
                        new Area{Name = "DUMMY AREA", ShortName = "DUMMY AREA", Code = "DUMMY"}
                    };

                Areas = areas.Cast<IArea>().ToList();
            }
            else if (areaTypeId > CategoryAreaType.IdAddition)
            {
                // e.g All categories for a category type
                int categoryTypeId = CategoryAreaType.GetCategoryTypeIdFromAreaTypeId(areaTypeId);
                Areas = areasReader.GetCategories(categoryTypeId)
                    .Select(CategoryArea.New).Cast<IArea>().ToList();
            }
            else
            {
                IList<string> parentCodes = areasReader.GetProfileParentAreaCodes(profileId, areaTypeId);

                IList<IArea> areas = parentCodes.Any()
                    ? areasReader.GetAreasFromCodes(parentCodes)
                    : areasReader.GetAreasByAreaTypeId(areaTypeId);

                Areas = areas.Cast<IArea>().ToList();
            }
        }

        public void CreateAreaListFromNearestNeighbourAreaCode(int profileId, string parentAreaCode)
        {
            var nearestNeighbourArea = new NearestNeighbourArea(parentAreaCode);
            var nearestNeighbours = areasReader.GetNearestNeighbours(nearestNeighbourArea.AreaCodeOfAreaWithNeighbours, nearestNeighbourArea.NeighbourTypeId);
            var nearestNeighboursAreas = nearestNeighbours.Select(x => x.NeighbourAreaCode).ToList();
            var areas = areasReader.GetAreasFromCodes(nearestNeighboursAreas);
            var sortedAreas = new List<IArea>();
            // Sort list of NN by their rank.
            foreach (var nearestNeighbourAreaCode in nearestNeighboursAreas)
            {
                var temp = areas.Where(x => x.Code == nearestNeighbourAreaCode).FirstOrDefault();
                sortedAreas.Add(temp);
            }
            Areas = sortedAreas;
        }

        public virtual void CreateChildAreaList(string parentAreaCode, int childAreaTypeId)
        {
            Areas = new ChildAreaListBuilder(areasReader).GetChildAreas(parentAreaCode, childAreaTypeId);
        }

        public virtual void RemoveAreasIgnoredEverywhere(IgnoredAreasFilter ignoredAreasFilter)
        {
            CheckAreasDefined();
            Areas = ignoredAreasFilter.RemoveAreasIgnoredEverywhere(Areas).ToList();
        }

        public virtual void SortByOrderOrName()
        {
            CheckAreasDefined();

            if (Areas.Any() == false) return;

            if (Areas.First().Sequence.HasValue)
            {
                Areas = Areas.OrderBy(x => x.Sequence).ToList();
            }
            else
            {
                Areas = Areas.OrderBy(x => x.Name).ToList();
            }
        }

        private void CheckAreasDefined()
        {
            if (Areas == null)
            {
                throw new FingertipsException("Area list must be created before this can be called");
            }
        }
    }
}