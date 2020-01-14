using Fpm.ProfileData;
using System.Linq;

namespace Fpm.MainUI.Helpers
{
    public class FileNameHelper
    {
        public enum Uniqueness
        {
            NotUnique,
            UniqueToProfile,
            Unique
        }

        private readonly IProfilesReader _reader;

        public FileNameHelper(IProfilesReader reader)
        {
            _reader = reader;
        }

        public Uniqueness IsUnique(string filename, int profileId)
        {
            var fileNameUniqueness = Uniqueness.NotUnique;
            var docs = _reader.GetDocumentsWithoutFileData(filename);

            if (docs.Count > 0)
            {
                // now check is this name used with current profile                
                var docExists = docs.FirstOrDefault(x => x.ProfileId == profileId);
                if (docExists != null)
                {
                    fileNameUniqueness = Uniqueness.UniqueToProfile;
                }
            }
            else
            {
                fileNameUniqueness = Uniqueness.Unique;
            }

            return fileNameUniqueness;
        }
    }
}