using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.ProfileData.Repositories
{
    public interface ICoreDataRepository
    {
        IEnumerable<CoreDataSetArchive> GetCoreDataArchives(string areaCode);
        void DeleteCoreDataArchive(Guid uploadBatchId);
        void DeleteCoreDataSet(Guid uploadBatchId);
        Area GetAreaDetail(string areaCode);
        void UpdateAreaDetail(Area newAreaDetail, string originalAreaCode, string loggedInUser);
        IEnumerable<CategoryType> GetCategoryTypes(int indicatorId);
        IEnumerable<AreaType> GetAreaTypes(int indicatorId);
        IEnumerable<Sex> GetSexes(int indicatorId);
        IEnumerable<Age> GetAges(int indicatorId);
        IEnumerable<int> GetYearRanges(int indicatorId);
        IEnumerable<int> GetYears(int indicatorId);
        IEnumerable<int> GetMonths(int indicatorId);
        IEnumerable<int> GetQuarters(int indicatorId);
        IEnumerable<CoreDataSet> GetCoreDataSet(int indicatorId, out int totalRows);
        IEnumerable<CoreDataSet> GetCoreDataSet(int indicatorId, Dictionary<string, object> filters, out int totalRows);
        int DeleteCoreDataSet(int indicatorId, Dictionary<string, object> filters, string userName);
        bool CanDeleteDataSet(int indicatorId, string userName);
        void DeleteCoreDataSet(int indicatorId, int valueNoteId);
    }
}