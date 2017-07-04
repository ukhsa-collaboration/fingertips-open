using System;
using System.Collections.Generic;
using System.Linq;
using Fpm.ProfileData.Entities.Profile;
using NHibernate;

namespace Fpm.ProfileData
{
    public class ProfilesWriter : ProfilesReader
    {
        public ProfilesWriter(ISessionFactory sessionFactory)
            : base(sessionFactory)
        { }

        public void ReorderIndicatorSequence(int groupId, int areaTypeId)
        {
            var groupings = GetGroupingsByGroupIdAndAreaTypeId(groupId, areaTypeId);
            new GroupingListProcessor().RecalculateSequences(groupings);
            UpdateGroupingList(groupings);
        }

        public void UpdateTargetConfig(TargetConfig target)
        {
            var fromDatabase = GetTargetById(target.Id);

            // Copy properties to object retrieved from the database
            fromDatabase.LowerLimit = target.LowerLimit;
            fromDatabase.UpperLimit = target.UpperLimit;
            fromDatabase.Description = target.Description;
            fromDatabase.PolarityId = target.PolarityId;
            fromDatabase.LegendHtml = target.LegendHtml;

            UpdateObject(fromDatabase);
        }

        public void UpdateGroupingMetadata(GroupingMetadata groupingMetadata)
        {
            GroupingMetadata fromDatabase = GetGroupingMetadata(groupingMetadata.GroupId);

            // Copy properties to object retrieved from the database
            fromDatabase.GroupName = groupingMetadata.GroupName;
            fromDatabase.Sequence = groupingMetadata.Sequence;

            UpdateObject(fromDatabase);
        }

        public void UpdateGroupingList(IList<Grouping> groupings)
        {
            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();
                foreach (var grouping in groupings)
                {
                    CurrentSession.Update(grouping);
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }

        public GroupingMetadata NewGroupingMetadata(string name, int sequence, int profileId)
        {
            var groupingMetadata = new GroupingMetadata
            {
                GroupName = name,
                Sequence = sequence,
                Description = name,
                ProfileId = profileId
            };

            var id = SaveNewObject(groupingMetadata);

            return GetGroupingMetadata(id);
        }

        public void DeleteOverriddenIndicatorMetadataTextValue(IndicatorMetadataTextValue indicatorMetadataTextValue)
        {
            var profileId = indicatorMetadataTextValue.ProfileId;

            // Profile will only be defined if overridden metadata is defined
            if (profileId.HasValue)
            {
                var fromDatabase = GetMetadataTextValueForAnIndicatorById(
                    indicatorMetadataTextValue.IndicatorId, profileId.Value);
                DeleteObject(fromDatabase);
            }
        }

        public int NewIndicatorMetadataTextValue(IndicatorMetadataTextValue indicatorMetadataTextValue)
        {
            var id = SaveNewObject(indicatorMetadataTextValue);
            return id;
        }

        public void DeleteTargetConfig(TargetConfig target)
        {
            DeleteObject(GetTargetById(target.Id));
        }

        public void NewTargetConfigAudit(TargetConfigAudit targetAudit)
        {
            SaveNewObject(targetAudit);
        }

        public TargetConfig NewTargetConfig(TargetConfig target)
        {
            var newTarget = new TargetConfig
            {
                Description = target.Description,
                LowerLimit = target.LowerLimit,
                UpperLimit = target.UpperLimit,
                PolarityId = target.PolarityId,
                LegendHtml = target.LegendHtml
            };

            var id = SaveNewObject(newTarget);
            return GetTargetById(id);
        }

        public ContentItem NewContentItem(int profileId, string contentKey, string description, bool isPlainTextContent, string content)
        {
            var contentItem = new ContentItem
            {
                ProfileId = profileId,
                Content = content,
                ContentKey = contentKey,
                Description = description,
                IsPlainText = isPlainTextContent
            };

            var id = SaveNewObject(contentItem);
            return GetContentItem(id);
        }

        public ContentAudit NewContentAudit(ContentAudit contentAudit)
        {
            var id = SaveNewObject(contentAudit);
            return GetContentAuditFromId(id);
        }

        public void UpdateContentItem(ContentItem contentItem)
        {
            var fromDatabase = GetContentItem(contentItem.Id);

            // Copy properties to object retrieved from the database
            fromDatabase.Content = contentItem.Content;
            fromDatabase.ContentKey = contentItem.ContentKey;
            fromDatabase.Description = contentItem.Description;
            fromDatabase.IsPlainText = contentItem.IsPlainText;

            UpdateObject(fromDatabase);
        }

        public void SetGroupingSequence(int groupId, int areaTypeId, int indicatorId, int sexId, int sequence)
        {

        }

        public void DeleteContentItem(string contentKey, int profileId)
        {
            var contentItem = GetContentItem(contentKey, profileId);
            DeleteObject(contentItem);
        }

        public void DeleteGroupingMetadata(int groupId)
        {
            DeleteObject(GetGroupingMetadata(groupId));
        }

        public void ReorderDomainSequence(IList<int> groupIds)
        {
            var groupingMetadataList = GetGroupingMetadataList(groupIds.ToList());

            var sequenceNumber = 1;
            foreach (var groupingMetadata in groupingMetadataList)
            {
                groupingMetadata.Sequence = sequenceNumber++;
            }

            UpdateObjectList(groupingMetadataList.Cast<object>().ToList());
        }

        public void DeleteIndicatorMetatdataById(IndicatorMetadata indicatorMetadata)
        {
            if (indicatorMetadata == null)
            {
                return;
            }

            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();

                // Delete indicator metadata
                CurrentSession.Delete(indicatorMetadata);

                // Delete core data
                var queryString = "delete from CoreDataSet d where d.IndicatorId = :indicatorId";
                IQuery q = CurrentSession.CreateQuery(queryString);
                q.SetParameter("indicatorId", indicatorMetadata.IndicatorId);
                q.ExecuteUpdate();

                // Delete text values
                var sqlQuery = CurrentSession.CreateSQLQuery(
                    "delete from IndicatorMetadataTextValues where indicatorid = " +
                    indicatorMetadata.IndicatorId);
                sqlQuery.ExecuteUpdate();

                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }

        private void DeleteObject(object objectToDelete)
        {
            if (objectToDelete != null)
            {
                ITransaction transaction = null;
                try
                {
                    transaction = CurrentSession.BeginTransaction();
                    CurrentSession.Delete(objectToDelete);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    if (transaction != null && transaction.WasRolledBack == false)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
            }
        }

        private int SaveNewObject(object objectToSave)
        {
            int id;
            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();
                id = (int)CurrentSession.Save(objectToSave);
                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
            return id;
        }

        private void UpdateObject(object objectToUpdate)
        {
            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();
                CurrentSession.Update(objectToUpdate);
                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }

        private void UpdateObjectList(List<object> objectsToUpdate)
        {
            ITransaction transaction = null;
            try
            {
                transaction = CurrentSession.BeginTransaction();
                foreach (var objectToUpdate in objectsToUpdate)
                {
                    CurrentSession.Update(objectToUpdate);
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }

        public int NewDocument(Document doc)
        {
            var id = SaveNewObject(doc);
            return id;
        }

        public void UpdateDocument(Document doc)
        {
            UpdateObject(doc);
        }

        public void DeleteDocument(Document doc)
        {
            DeleteObject(doc);
        }
    }
}
