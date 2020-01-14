
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class GroupRootBuilder
    {
        // set required: Automatically implemented properties must define both get and set accessors
        private IList<GroupRoot> GroupRoots { get; set; }

        private GroupRoot root;
        private readonly bool _isCommonestPolarity;
        private readonly IGroupDataReader _groupDataReader;

        public GroupRootBuilder(IGroupDataReader groupDataReader)
        {
            GroupRoots = new List<GroupRoot>();
            _isCommonestPolarity = false;
            _groupDataReader = groupDataReader;
        }

        public GroupRootBuilder(int profileId, IGroupDataReader groupDataReader)
        {
            GroupRoots = new List<GroupRoot>();
            _isCommonestPolarity = PolarityHelper.ShouldUseCommonestPolarity(profileId);
            _groupDataReader = groupDataReader;
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
                    // State sex
                    bool stateSex;
                    var alwaysShowSexWithIndicatorName =
                        _groupDataReader.GetIndicatorMetadataAlwaysShowSexWithIndicatorName(indicatorId);

                    if (alwaysShowSexWithIndicatorName)
                    {
                        stateSex = true;
                    }
                    else
                    {
                        stateSex = groupingsWithSameIndicatorId.Select(x => x.SexId).Distinct().Count() > 1;
                    }

                    // State age
                    bool stateAge;
                    var alwaysShowAgeWithIndicatorName =
                        _groupDataReader.GetIndicatorMetadataAlwaysShowAgeWithIndicatorName(indicatorId);
                    if (alwaysShowAgeWithIndicatorName)
                    {
                        stateAge = true;
                    }
                    else
                    {
                        stateAge = groupingsWithSameIndicatorId.Select(x => x.AgeId).Distinct().Count() > 1;
                    }

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

            SetSequences();

            return GroupRoots;
        }

        /// <summary>
        /// Set the sequence of each group root in the list
        /// </summary>
        private void SetSequences()
        {
            var sequence = 1;
            foreach (var groupRoot in GroupRoots)
            {
                groupRoot.Sequence = sequence++;
            }
        }

        private void CreateNewRoot(List<Grouping> groupingsWithSameIndicatorIdSexIdAndAgeId, bool stateSex, bool stateAge)
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
            SetPolarityId(root.IndicatorId);
            root.StateSex = stateSex;
            root.StateAge = stateAge;
        }

        private void AssignPropertiesFromGrouping(Grouping grouping)
        {
            root.PolarityId = grouping.PolarityId;
            root.SexId = grouping.SexId;
            root.AgeId = grouping.AgeId;
            root.ComparatorMethodId = grouping.ComparatorMethodId;
            root.YearRange = grouping.YearRange;
            root.Sex = grouping.Sex;
            root.Age = grouping.Age;
        }

        private void AssignGroupings(IEnumerable<Grouping> groupings)
        {
            foreach (Grouping grouping in groupings)
            {
                root.Add(grouping);
            }
        }

        private void SetPolarityId(int indicatorId)
        {
            if (_isCommonestPolarity)
            {
                var polarityId = PolarityHelper.GetCommonestPolarityForIndicator(indicatorId);

                // Ensure groupings have consistent polarity with the root
                root.PolarityId = polarityId;
                foreach (var groupingA in root.Grouping)
                {
                    groupingA.PolarityId = polarityId;
                }
            } 
        }
    }
}