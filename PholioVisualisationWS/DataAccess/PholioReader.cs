using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public class PholioReader : BaseReader
    {
        private const string PropertyId = "Id";

        /// <summary>
        ///     Parameterless constructor to allow mocking
        /// </summary>
        public PholioReader()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        internal PholioReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public IList<string> GetQuinaryPopulationLabels()
        {
            IQuery q = CurrentSession.GetNamedQuery("GetQuinaryPopulationLabels");
            IList<string> labels = q.List().Cast<string>().ToList();

            // Sort labels
            var sortMap = new Dictionary<int, string>();
            foreach (string label in labels)
            {
                string number = label.Split('-', '+')[0];
                sortMap[int.Parse(number)] = label;
            }

            var keys = new List<int>(sortMap.Keys);
            keys.Sort();
            labels = keys.Select(key => sortMap[key]).ToList();

            return labels.Select(x => x.Replace("yrs", string.Empty).Trim()).ToList();
        }

        public virtual Dictionary<string, double> GetCcgPracticePopulations(string parentAreaCode)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetCcgPracticePopulations");
            q.SetParameter("parentAreaCode", parentAreaCode);
            q.SetParameter("populationIndicatorId", IndicatorIds.QofListSize);
            IList results = q.List();

            return results
                .Cast<IList<object>>()
                .ToDictionary(itemArray => (string) itemArray[0], itemArray => (double) itemArray[1]);
        }

        public virtual TargetConfig GetTargetConfig(int targetComparerId)
        {
            return CurrentSession.CreateCriteria<TargetConfig>()
                .Add(Restrictions.Eq(PropertyId, targetComparerId))
                .UniqueResult<TargetConfig>();
        }

        public virtual IList<ValueNote> GetValueNotes()
        {
            return CurrentSession.CreateCriteria<ValueNote>()
                .List<ValueNote>();
        }

        public virtual Age GetAgeById(int ageId)
        {
            return GetAgesByIds(new List<int>{ageId}).First();
        }

        public virtual IList<Age> GetAgesByIds(IList<int> ageIds)
        {
            return CurrentSession.CreateCriteria<Age>()
                .Add(Restrictions.In("Id", ageIds.ToList()))
                .List<Age>();
        }

        public virtual IList<Age> GetAllAges()
        {
            return CurrentSession.CreateCriteria<Age>()
                .List<Age>();
        }

        public virtual IList<Sex> GetSexesByIds(IList<int> sexIds)
        {
            return CurrentSession.CreateCriteria<Sex>()
                .Add(Restrictions.In("Id", sexIds.ToList()))
                .AddOrder(Order.Asc("Sequence"))
                .List<Sex>();
        }

        public virtual IList<Sex> GetAllSexes()
        {
            return CurrentSession.CreateCriteria<Sex>()
                .AddOrder(Order.Asc("Sequence"))
                .List<Sex>();
        }

        public virtual IList<KeyMessageOverride> GetKeyMessageOverrides(int profileId, string areaCode)
        {
            return CurrentSession.CreateCriteria<KeyMessageOverride>()
                .Add(Restrictions.Eq("ProfileId", profileId))
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .List<KeyMessageOverride>();
        }

        public virtual ComparatorConfidence GetComparatorConfidence(int comparatorMethodId, double comparatorConfidence)
        {
            var result = CurrentSession.CreateCriteria<ComparatorConfidence>()
                .Add(Restrictions.Eq("ComparatorMethodId", comparatorMethodId))
                .Add(Restrictions.Eq("ConfidenceValue", comparatorConfidence))
                .UniqueResult<ComparatorConfidence>();

            if (result == null)
            {
                throw new FingertipsException(string.Format(
                    "Invalid comparator method ID ({0}) and comparator confidence ({1}) combination", comparatorMethodId,
                    comparatorConfidence
                    ));
            }

            return result;
        }
    }
}