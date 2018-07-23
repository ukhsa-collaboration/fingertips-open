using System;
using System.Text.RegularExpressions;
using IndicatorsUI.UserAccess.UserList.IRepository;

namespace IndicatorsUI.MainUI.Helpers
{
    public interface IPublicIdGenerator
    {
        string GetIndicatorListPublicId();
    }

    public class PublicIdGenerator : IPublicIdGenerator
    {
        public const int UidLength = 10;

        private IIndicatorListRepository _indicatorListRepository;

        public PublicIdGenerator(IIndicatorListRepository indicatorListRepository)
        {
            _indicatorListRepository = indicatorListRepository;
        }

        public string GetIndicatorListPublicId()
        {
            var uid = GetShortUid();

            // Ensure UID is unique
            while (_indicatorListRepository.GetListByPublicId(uid) != null)
            {
                uid = GetShortUid();
            }

            return uid;
        }

        private static string GetShortUid()
        {
            var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")
                .Substring(0, UidLength);
            return uid;
        }
    }
}