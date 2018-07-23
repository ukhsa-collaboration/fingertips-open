using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IndicatorsUI.UserAccess.UserList.IRepository;

namespace IndicatorsUI.UserAccess.UserList.Repository
{
    public class IndicatorListRepo : IIndicatorListRepository
    {
        private FINGERTIPS_USERSEntities _dbContext;

        public IndicatorListRepo(FINGERTIPS_USERSEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public bool DoesUserOwnList(string publicListId, string userId)
        {
            var indicatorListFromDb = GetListByPublicId(publicListId);
            return indicatorListFromDb.UserId == userId;
        }

        public IndicatorList GetListByPublicId(string publicId)
        {
            return _dbContext.IndicatorLists
                .Include("IndicatorListItems")
                .FirstOrDefault(x => x.PublicId == publicId);
        }

        public string GetListNameByPublicId(string publicId)
        {
            return _dbContext.IndicatorLists
                .Where(x => x.PublicId == publicId)
                .Select(x => x.ListName)
                .FirstOrDefault();
        }

        public IndicatorList Get(int id)
        {

            return _dbContext.IndicatorLists
                .Include("IndicatorListItems").FirstOrDefault(x => x.Id == id);
        }

        public int Save(IndicatorList entity)
        {

            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            _dbContext.IndicatorLists.Add(entity);
            _dbContext.SaveChanges();
            return entity.Id;
        }

        public void Delete(int id)
        {
            var indicator = _dbContext.IndicatorLists.Include("IndicatorListItems").FirstOrDefault(x => x.Id == id);
            if (indicator != null)
            {
                _dbContext.IndicatorLists.Remove(indicator);
                _dbContext.SaveChanges();
            }
        }

        public void Update(IndicatorList indicatorList)
        {
            var indicatorListFromDb = _dbContext.IndicatorLists.Include("IndicatorListItems")
                .FirstOrDefault(x => x.Id == indicatorList.Id);
            if (indicatorListFromDb != null)
            {
                indicatorListFromDb.UpdatedOn = DateTime.Now;
                indicatorListFromDb.ListName = indicatorList.ListName;

                // Replace list items
                _dbContext.IndicatorListItems.RemoveRange(indicatorListFromDb.IndicatorListItems);
                indicatorListFromDb.IndicatorListItems = indicatorList.IndicatorListItems;

                _dbContext.Entry(indicatorListFromDb).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<IndicatorList> GetTopIndicatorList(int listCount, string userId)
        {
            return _dbContext.IndicatorLists
                .Where(d => d.UserId == userId)
                .OrderBy(d => d.ListName)
                .Take(listCount)
                .ToList();
        }
        public IEnumerable<IndicatorList> GetAll(string userId)
        {
            return _dbContext.IndicatorLists
                .Where(d => d.UserId == userId)
                .OrderByDescending(o => o.UpdatedOn)
                .ToList();
        }
    }
}
