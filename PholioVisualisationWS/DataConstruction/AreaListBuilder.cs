using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AreaListBuilder
    {
        public virtual List<IArea> Areas { get; private set; }

        private IAreasReader areasReader;

        /// <summary>
        /// Parameterless constructor to enable mocking
        /// </summary>
        public AreaListBuilder() { }

        public AreaListBuilder(IAreasReader areasReader)
        {
            this.areasReader = areasReader;
        }

        public void CreateAreaListFromAreaCodes(IEnumerable<string> areaCodes)
        {
            Areas = areaCodes.Select(areaCode => AreaFactory.NewArea(areasReader, areaCode)).ToList();
        }

        public void CreateAreaListFromAreaTypeId(int profileId, int areaTypeId)
        {
            if (areaTypeId > NeighbourAreaType.IdAddition) //TODO: need to remove this 
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

                IList<Area> areas = parentCodes.Any()
                    ? areasReader.GetAreasFromCodes(parentCodes)
                    : areasReader.GetAreasByAreaTypeId(areaTypeId);

                Areas = areas.Cast<IArea>().ToList();
            }
        }

        public void CreateAreaListFromNearestNeighbourAreaCode(int profileId, string parentAreaCode)
        {
            var areaCode = new NearestNeighbourArea(parentAreaCode).AreaCodeOfAreaWithNeighbours;
            var nearestNeighbours = areasReader.GetNearestNeighbours(areaCode);
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
            Areas = new ChildAreaListBuilder(areasReader, parentAreaCode, childAreaTypeId).ChildAreas;
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