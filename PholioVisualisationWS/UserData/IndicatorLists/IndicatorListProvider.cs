using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.UserData.IndicatorLists
{
    public class IndicatorListProvider
    {
        private IIndicatorListRepository _indicatorListRepository;
        private PholioReader _pholioReader;
        private IIndicatorMetadataRepository _indicatorMetadataRepository;

        public IndicatorListProvider(IIndicatorListRepository indicatorListRepository,
            PholioReader pholioReader, IIndicatorMetadataRepository indicatorMetadataRepository)
        {
            _indicatorListRepository = indicatorListRepository;
            _pholioReader = pholioReader;
            _indicatorMetadataRepository = indicatorMetadataRepository;
        }

        public IndicatorList GetIndicatorList(string publicId)
        {
            var list = _indicatorListRepository.GetIndicatorList(publicId);
            if (list != null)
            {
                var listItems = list.IndicatorListItems;

                // Get look ups
                var indicatorIdToName = GetIndicatorNameLookUp(listItems);
                var ageIdToAge = GetAges(listItems);
                var sexIdToSex = GetSexes(listItems);

                IList<IndicatorListItem> itemsToRemove = new List<IndicatorListItem>();
                foreach (var listItem in listItems)
                {
                    var indicatorId = listItem.IndicatorId;
                    if (indicatorIdToName.ContainsKey(indicatorId))
                    {
                        // Add non-persisted properties
                        listItem.Sex = sexIdToSex[listItem.SexId];
                        listItem.Age = ageIdToAge[listItem.AgeId];
                        listItem.IndicatorName = indicatorIdToName[indicatorId];
                    }
                    else
                    {
                        // Do not include items if the indicator does not exist
                        itemsToRemove.Add(listItem);
                    }
                }

                // Remove items with deleted indicators
                var cleanList = listItems.ToList();
                cleanList.RemoveAll(x => itemsToRemove.Contains(x));
                list.IndicatorListItems = cleanList;
            }

            return list;
        }

        private Dictionary<int, string> GetIndicatorNameLookUp(ICollection<IndicatorListItem> listItems)
        {
            var indicatorIdToName = _indicatorMetadataRepository
                .GetIndicatorNames(listItems.Select(x => x.IndicatorId))
                .ToDictionary(x => x.Id, x => x.Name);
            return indicatorIdToName;
        }

        private Dictionary<int, Sex> GetSexes(ICollection<IndicatorListItem> listItems)
        {
            var sexIds = listItems.Select(x => x.SexId).ToList();
            var sexes = _pholioReader.GetSexesByIds(sexIds).ToDictionary(x => x.Id, x => x);
            return sexes;
        }

        private Dictionary<int, Age> GetAges(ICollection<IndicatorListItem> listItems)
        {
            var ageIds = listItems.Select(x => x.AgeId).ToList();
            var ages = _pholioReader.GetAgesByIds(ageIds).ToDictionary(x => x.Id, x => x);
            return ages;
        }
    }
}
