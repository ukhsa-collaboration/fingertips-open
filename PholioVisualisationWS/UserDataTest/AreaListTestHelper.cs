using System;
using System.Collections.Generic;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.UserDataTest
{
    public class AreaListTestHelper
    {
        public const string UserId = FingertipsUserIds.TestUser;
        public const string PublicId = AreaListCodes.TestListId;

        private IAreaListRepository _areaListRepository;
        private int _areaListId;

        public AreaListTestHelper()
        {
            var dbContext = new fingertips_usersEntities();
            _areaListRepository = new AreaListRepository(dbContext);
        }

        public int CreateTestList(IList<string> areaCodes)
        {
            var areaList = new AreaList
            {
                AreaTypeId = AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019,
                UserId = UserId,
                ListName = "TestAreaList " + PublicId,
                PublicId = PublicId,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now
            };

            _areaListId = _areaListRepository.Save(areaList, areaCodes);
            return _areaListId;
        }

        public void DeleteTestList()
        {
            _areaListRepository.Delete(_areaListId);
        }
    }
}