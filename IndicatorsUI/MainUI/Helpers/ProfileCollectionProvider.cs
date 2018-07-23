using System.Collections.Generic;
using System.Web;
using IndicatorsUI.DataAccess;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.MainUI.Helpers
{
    public static class ProfileCollectionProvider
    {
        private static ProfileReader profileReader = ReaderFactory.GetProfileReader();

        public static ProfileCollection GetProfileCollection(int skinId)
        {
            return profileReader.GetProfileCollection(skinId);
        }

        public static IList<SkinProfileCollection> GetSkinProfileCollections(int skinId)
        {
            return profileReader.GetSkinProfileCollections(skinId);
        }

        public static IList<ProfileCollectionItem> GetProfileCollectionItems(int profileCollectionId)
        {
            return profileReader.GetProfileCollectionItems(profileCollectionId);
        }

    }
}