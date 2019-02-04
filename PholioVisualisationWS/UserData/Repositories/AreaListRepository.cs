using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.UserData.Repositories
{
    public class AreaListRepository : IAreaListRepository, IDisposable
    {
        private readonly fingertips_usersEntities _dbContext;

        public AreaListRepository(fingertips_usersEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public bool DoesUserOwnList(string publicId, string userId)
        {
            var areaListFromDb = GetAreaListByPublicId(publicId);
            return areaListFromDb.UserId == userId;
        }

        public AreaList Get(int id)
        {
            return _dbContext.AreaLists.Include("AreaListAreaCodes")
                .FirstOrDefault(x => x.Id == id);
        }

        public AreaList GetAreaListByPublicId(string publicId)
        {
            return _dbContext.AreaLists.Include("AreaListAreaCodes")
                .FirstOrDefault(x => x.PublicId == publicId);
        }

        public string GetUserIdByPublicId(string publicId)
        {
            var areaListFromDb = GetAreaListByPublicId(publicId);
            return areaListFromDb.UserId;
        }

        public IEnumerable<AreaList> GetAll(string userId)
        {
            return _dbContext.AreaLists.Include("AreaListAreaCodes")
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UpdatedOn)
                .ToList();
        }

        public IEnumerable<AreaListAreaCode> GetAreaListAreaCodes(int areaListId)
        {
            return _dbContext.AreaListAreaCodes
                .Where(x => x.AreaListId == areaListId)
                .OrderBy(x => x.AreaCode)
                .ToList();
        }

        public int Save(AreaList areaList, IList<string> areaCodes)
        {
            _dbContext.AreaLists.Add(areaList);

            foreach (string areaCode in areaCodes)
            {
                AreaListAreaCode areaListAreaCode = new AreaListAreaCode
                {
                    AreaListId = areaList.Id,
                    AreaCode = areaCode
                };

                _dbContext.AreaListAreaCodes.Add(areaListAreaCode);
            }

            _dbContext.SaveChanges();
            return areaList.Id;
        }

        public void Update(int areaListId, string areaListName, IList<string> areaCodes)
        {
            var areaListFromDb = _dbContext.AreaLists.Include("AreaListAreaCodes").First(x => x.Id == areaListId);

            if (areaListFromDb != null)
            {
                areaListFromDb.UpdatedOn = DateTime.Now;
                areaListFromDb.ListName = areaListName;

                _dbContext.AreaListAreaCodes.RemoveRange(areaListFromDb.AreaListAreaCodes);

                foreach (string areaCode in areaCodes)
                {
                    AreaListAreaCode areaListAreaCode = new AreaListAreaCode
                    {
                        AreaListId = areaListId,
                        AreaCode = areaCode
                    };

                    areaListFromDb.AreaListAreaCodes.Add(areaListAreaCode);
                }

                _dbContext.Entry(areaListFromDb).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var areaList = _dbContext.AreaLists.Include("AreaListAreaCodes")
                .FirstOrDefault(x => x.Id == id);

            if (areaList != null)
            {
                _dbContext.AreaListAreaCodes.RemoveRange(areaList.AreaListAreaCodes);
                _dbContext.AreaLists.Remove(areaList);
                _dbContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
