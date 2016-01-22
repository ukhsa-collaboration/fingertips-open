using System.Text.RegularExpressions;

namespace Ckan.DataTransformation
{
    public class CkanFileNamer
    {
        // CKAN will truncate filenames longer than 100 characters
        private const int CharacterLimit = 80;

        private readonly string indicatorName;

        public CkanFileNamer(string indicatorName)
        {
            this.indicatorName = indicatorName;
        }

        public string MetadataFileName
        {
            get { return string.Format("{0}.metadata.csv", GetName()); }
        }

        public string DataFileName
        {
            get { return string.Format("{0}.data.csv", GetName()); }
        }

        private string GetName()
        {
            string name = new Regex("[^A-Za-z0-9]").Replace(indicatorName,"");

            if (name.Length > CharacterLimit)
            {
                name = name.Substring(0, CharacterLimit);
            }
            return name;
        }
    }
}