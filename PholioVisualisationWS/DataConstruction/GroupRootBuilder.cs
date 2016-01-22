﻿
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootBuilder
    {
        private IList<GroupRoot> GroupRoots { get; set; }
        private GroupRoot root;

        private PholioReader pholioReader = ReaderFactory.GetPholioReader();

        public GroupRootBuilder()
        {
            GroupRoots = new List<GroupRoot>();
        }

        public IList<GroupRoot> BuildGroupRoots(IList<Grouping> groupings)
        {
            // Select from groupingsWithSameIndicatorIdSexIdAndAgeId to preserve Sequence ordering
            IEnumerable<int> indicatorIds = (from g in groupings select g.IndicatorId).Distinct();

            foreach (int indicatorId in indicatorIds)
            {
                var groupingsWithSameIndicatorId = groupings.Where(g => g.IndicatorId == indicatorId).ToList();

                if (groupingsWithSameIndicatorId.Any())
                {
                    var stateSex = groupingsWithSameIndicatorId.Select(x => x.SexId).Distinct().Count() > 1;
                    var stateAge = groupingsWithSameIndicatorId.Select(x => x.AgeId).Distinct().Count() > 1;

                    var groupingsPerRoot = groupingsWithSameIndicatorId
                        .GroupBy(g => new {
                            g.SexId,
                            g.AgeId,
                            IsDataQuarterly = g.IsDataQuarterly(),
                            IsDataMonthly = g.IsDataMonthly()
                        });

                    foreach (var groupingsForOneRoot in groupingsPerRoot)
                    {
                        CreateNewRoot(groupingsForOneRoot.ToList(), stateSex, stateAge);
                    }
                }
            }

            return GroupRoots;
        }

        private void CreateNewRoot(List<Grouping> groupingsWithSameIndicatorIdSexIdAndAgeId,
            bool stateSex, bool stateAge)
        {
            // Get distinct comparator groupings (1 & 4) for these indicators
            var comparatorIds = groupingsWithSameIndicatorIdSexIdAndAgeId.Select(x => x.ComparatorId).Distinct();

            var templateGrouping = groupingsWithSameIndicatorIdSexIdAndAgeId.First();
            var newGroupings = new List<Grouping>();
            foreach (var comparatorId in comparatorIds)
            {
                var grouping = new Grouping(templateGrouping) {ComparatorId = comparatorId};
                newGroupings.Add(grouping);
            }

            root = new GroupRoot();
            GroupRoots.Add(root);
            AssignGroupings(newGroupings);
            AssignPropertiesFromGrouping(newGroupings.First());
            root.StateSex = stateSex;

            if (stateAge)
            {
                root.AgeLabel = pholioReader.GetAgeById(newGroupings.First().AgeId).Name;
            }
        }

        private void AssignPropertiesFromGrouping(Grouping grouping)
        {
            root.PolarityId = grouping.PolarityId;
            root.SexId = grouping.SexId;
            root.AgeId = grouping.AgeId;
            root.ComparatorMethodId = grouping.ComparatorMethodId;
        }

        private void AssignGroupings(IEnumerable<Grouping> groupings)
        {
            foreach (Grouping grouping in groupings)
            {
                root.Add(grouping);
            }
        }
    }
}