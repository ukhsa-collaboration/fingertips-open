using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public interface IPholioReader
    {
        IList<string> GetQuinaryPopulationLabels(IList<int> ageIds);
        Dictionary<string, double> GetCcgPracticePopulations(string parentAreaCode);
        TargetConfig GetTargetConfig(int targetComparerId);
        IList<ValueNote> GetAllValueNotes();
        IList<YearType> GetAllYearTypes();
        YearType GetYearType(int id);
        IList<Unit> GetAllUnits();
        IList<Polarity> GetAllPolarities();
        IList<ValueType> GetAllValueTypes();
        IList<ComparatorMethod> GetAllComparatorMethods();
        ComparatorMethod GetComparatorMethod(int id);
        IList<Comparator> GetAllComparators();
        Age GetAgeById(int ageId);
        IList<Age> GetAgesByIds(IList<int> ageIds);
        IList<Age> GetAllAges();
        IList<int> GetAllAgeIds();
        IList<Sex> GetSexesByIds(IList<int> sexIds);
        IList<Sex> GetAllSexes();
        ConfidenceIntervalMethod GetConfidenceIntervalMethod(int id);
        IList<ConfidenceIntervalMethod> GetAllConfidenceIntervalMethods();
        IList<KeyMessageOverride> GetKeyMessageOverrides(int profileId, string areaCode);
        ComparatorConfidence GetComparatorConfidence(int comparatorMethodId, double comparatorConfidence);
        IList<object> GetExceededOverriddenIndicatorMetadataTextValues();
        IList<NearestNeighbourType> GetAllNearestNeighbourTypes();
        NearestNeighbourType GetNearestNeighbourType(int neighbourTypeId);

        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="System.Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }

    public class PholioReader : BaseReader, IPholioReader
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

        public IList<string> GetQuinaryPopulationLabels(IList<int> ageIds)
        {
            var labels = GetAgesByIds(ageIds).Select(x => x.Name);

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
                .SetCacheable(true)
                .Add(Restrictions.Eq(PropertyId, targetComparerId))
                .UniqueResult<TargetConfig>();
        }

        public virtual IList<ValueNote> GetAllValueNotes()
        {
            return CurrentSession.CreateCriteria<ValueNote>()
                .SetCacheable(true)
                .List<ValueNote>();
        }

        public virtual IList<YearType> GetAllYearTypes()
        {
            return CurrentSession.CreateCriteria<YearType>()
                .SetCacheable(true)
                .List<YearType>();
        }

        public virtual YearType GetYearType(int id)
        {
            return CurrentSession.CreateCriteria<YearType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<YearType>();
        }

        public virtual IList<Unit> GetAllUnits()
        {
            return CurrentSession.CreateCriteria<Unit>()
                .SetCacheable(true)
                .List<Unit>();
        }

        public virtual IList<Polarity> GetAllPolarities()
        {
            return CurrentSession.CreateCriteria<Polarity>()
                .SetCacheable(true)
                .List<Polarity>();
        }

        public virtual IList<ValueType> GetAllValueTypes()
        {
            return CurrentSession.CreateCriteria<ValueType>()
                .SetCacheable(true)
                .List<ValueType>();
        }

        public virtual IList<ComparatorMethod> GetAllComparatorMethods()
        {
            return CurrentSession.CreateCriteria<ComparatorMethod>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("IsCurrent", true))
                .List<ComparatorMethod>();
        }

        public virtual ComparatorMethod GetComparatorMethod(int id)
        {
            return CurrentSession.CreateCriteria<ComparatorMethod>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<ComparatorMethod>();
        }

        public virtual IList<Comparator> GetAllComparators()
        {
            return CurrentSession.CreateCriteria<Comparator>()
                .SetCacheable(true)
                .List<Comparator>();
        }

        public virtual Age GetAgeById(int ageId)
        {
            return GetAgesByIds(new List<int>{ageId}).First();
        }

        public virtual IList<Age> GetAgesByIds(IList<int> ageIds)
        {
            return CurrentSession.CreateCriteria<Age>()
                .SetCacheable(true)
                .Add(Restrictions.In("Id", ageIds.ToList()))
                .List<Age>();
        }

        public virtual IList<Age> GetAllAges()
        {
            return CurrentSession.CreateCriteria<Age>()
                .SetCacheable(true)
                .List<Age>();
        }

        public IList<int> GetAllAgeIds()
        {
            return CurrentSession.CreateCriteria<Age>()
                .SetCacheable(true)
                .SetProjection(Projections.Property("Id"))
                .List<int>();
        }

        public virtual IList<Sex> GetSexesByIds(IList<int> sexIds)
        {
            return CurrentSession.CreateCriteria<Sex>()
                .SetCacheable(true)
                .Add(Restrictions.In("Id", sexIds.ToList()))
                .AddOrder(Order.Asc("Sequence"))
                .List<Sex>();
        }

        public virtual IList<Sex> GetAllSexes()
        {
            return CurrentSession.CreateCriteria<Sex>()
                .SetCacheable(true)
                .AddOrder(Order.Asc("Sequence"))
                .List<Sex>();
        }

        public ConfidenceIntervalMethod GetConfidenceIntervalMethod(int id)
        {
            return CurrentSession.CreateCriteria<ConfidenceIntervalMethod>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", id))
                .UniqueResult<ConfidenceIntervalMethod>();
        }

        public IList<ConfidenceIntervalMethod> GetAllConfidenceIntervalMethods()
        {
            return CurrentSession.CreateCriteria<ConfidenceIntervalMethod>()
                .SetCacheable(true)
                .List<ConfidenceIntervalMethod>();
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
                .SetCacheable(true)
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
        
        public IList<object> GetExceededOverriddenIndicatorMetadataTextValues()
        {
            var q = CurrentSession.GetNamedQuery("GetExceededOverriddenIndicatorMetadataTextValues");
            return q.List<object>();
        }

        public IList<NearestNeighbourType> GetAllNearestNeighbourTypes()
        {
            return CurrentSession.CreateCriteria<NearestNeighbourType>()
                .SetCacheable(true)
                .List<NearestNeighbourType>();
        }

        public NearestNeighbourType GetNearestNeighbourType(int neighbourTypeId)
        {
            return CurrentSession.CreateCriteria<NearestNeighbourType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("NeighbourTypeId", neighbourTypeId))
                .UniqueResult<NearestNeighbourType>();
        }
    }
}