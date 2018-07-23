using System.Text.RegularExpressions;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.Export.File
{
    public class SingleEntityFileNamer
    {
        public const string IndicatorMetadataFilenameForUser = "indicators.metadata.csv";
        public const string IndicatorDataFilenameForUser = "indicators.data.csv";

        // CKAN will truncate filenames longer than 100 characters
        private const int CharacterLimit = 80;

        private readonly string _entityName;

        public SingleEntityFileNamer(string entityName)
        {
            _entityName = entityName;
        }

        public string MetadataFileName
        {
            get { return string.Format("{0}.metadata.csv", GetName()); }
        }

        public string DataFileName
        {
            get { return string.Format("{0}.data.csv", GetName()); }
        }

        public string AddressesFileName
        {
            get { return string.Format("{0}.addresses.csv", GetName()); }
        }

        public static string GetAllMetadataFileNameForUser()
        {
            return new SingleEntityFileNamer("all-indicators").MetadataFileName;
        }

        public static string GetProfileMetadataFileNameForUser(int profileId)
        {
            var profileName = GetProfileName(profileId);
            return new SingleEntityFileNamer(profileName).MetadataFileName;
        }

        public static string GetDataForUserbyProfileAndAreaType(int profileId, int areaTypeId)
        {
            var profileName = GetProfileName(profileId);
            var areaTypeName = GetAreaTypeName(areaTypeId);
            var name = string.Format("{0}-{1}", profileName, areaTypeName);
            return new SingleEntityFileNamer(name).DataFileName;
        }

        public static string GetDataForUserbyIndicatorAndAreaType(int areaTypeId)
        {
            var areaTypeName = GetAreaTypeName(areaTypeId);
            var name = string.Format("indicators-{0}", areaTypeName);
            return new SingleEntityFileNamer(name).DataFileName;
        }

        private static string GetAreaTypeName(int areaTypeId)
        {
            var areaTypeName = ReaderFactory.GetAreasReader().GetAreaType(areaTypeId).ShortName;
            return new Regex("[^A-Za-z0-9]").Replace(areaTypeName, "");
        }

        private static string GetProfileName(int profileId)
        {
            var profileName = ReaderFactory.GetProfileReader().GetProfileConfig(profileId).Name;
            return profileName;
        }

        private string GetName()
        {
            string name = new Regex("[^A-Za-z0-9-]").Replace(_entityName,"");

            if (name.Length > CharacterLimit)
            {
                name = name.Substring(0, CharacterLimit);
            }
            return name;
        }
    }
}