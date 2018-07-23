using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.DataAccess.Repositories.Fpm.ProfileData.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class FpmGroupingRepository : RepositoryBase
    {
        public FpmGroupingRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public FpmGroupingRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        /// <summary>
        /// Method to replace the groupings for an indicator
        /// </summary>
        /// <param name="groupings">List of groupings</param>
        public void ReplaceGroupings(IList<Grouping> groupings)
        {
            // Check there are groupings
            if (groupings == null || groupings.Any() == false)
            {
                return;
            }

            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                List<int> groupIds = groupings.Select(g => g.GroupId).Distinct().ToList();
                int indicatorId = groupings[0].IndicatorId;

                // Delete the grouping corresponding to the indicator id
                IQuery q = CurrentSession.CreateQuery(
                    "delete from Grouping g where g.GroupId in (:groupId) and g.IndicatorId = :indicatorId");
                q.SetParameterList(BaseReader.ParameterGroupId, groupIds);
                q.SetParameter(BaseReader.ParameterIndicatorId, indicatorId);
                q.ExecuteUpdate();

                // Loop through the core data set and save them
                foreach (Grouping grouping in groupings)
                {
                    CurrentSession.GetNamedQuery("Insert_Groupings")
                        .SetParameter("GroupId", grouping.GroupId)
                        .SetParameter("IndicatorId", grouping.IndicatorId)
                        .SetParameter("AreaTypeId", grouping.AreaTypeId)
                        .SetParameter("SexId", grouping.SexId)
                        .SetParameter("AgeId", grouping.AgeId)
                        .SetParameter("ComparatorId", grouping.ComparatorId)
                        .SetParameter("ComparatorMethodId", grouping.ComparatorMethodId)
                        .SetParameter("ComparatorConfidence", grouping.ComparatorConfidence)
                        .SetParameter("YearRange", grouping.YearRange)
                        .SetParameter("BaselineYear", grouping.BaselineYear)
                        .SetParameter("BaselineQuarter", grouping.BaselineQuarter)
                        .SetParameter("BaselineMonth", grouping.BaselineMonth)
                        .SetParameter("DataPointYear", grouping.DataPointYear)
                        .SetParameter("DataPointQuarter", grouping.DataPointQuarter)
                        .SetParameter("DataPointMonth", grouping.DataPointMonth)
                        .SetParameter("Sequence", grouping.Sequence)
                        .SetParameter("ComparatorTargetId", grouping.ComparatorTargetId)
                        .SetParameter("PolarityId", grouping.PolarityId)
                        .ExecuteUpdate();
                }

                // All went well, commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                // Something wrong, rollback the transaction
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }

                // Throw the exception
                throw;
            }
        }

        /// <summary>
        /// Method to delete all groupings for a profile
        /// </summary>
        /// <param name="profileId">ProfileId</param>
        public void DeleteAllGroupingsForProfile(int profileId)
        {
            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                // Get the list of group id based on the profile id
                IList<int> groupIds = CurrentSession.CreateCriteria<GroupingMetadata>()
                    .Add(Restrictions.Eq("ProfileId", profileId))
                    .SetProjection(Projections.Property("Id"))
                    .List<int>();

                // Delete all groupings corresponding to the profile id
                IQuery q = CurrentSession.CreateQuery("delete from Grouping g where g.GroupId in (:groupId)");
                q.SetParameterList(BaseReader.ParameterGroupId, groupIds.ToList());
                q.ExecuteUpdate();

                // All went well, commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                // Something wrong, rollback the transaction
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }

                // Throw the exception
                throw;
            }
        }
    }
}
