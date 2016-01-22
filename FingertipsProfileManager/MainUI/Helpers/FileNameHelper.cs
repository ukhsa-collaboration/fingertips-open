using System.Linq;
using Fpm.ProfileData;

namespace Fpm.MainUI.Helpers
{

    public class FileNameHelper
    {
        private readonly ProfilesReader _reader = ReaderFactory.GetProfilesReader();
       
        public enum Uniqueness
        {
            NotUnique,
            UniqueToProfile,
            Unique
        }

        public Uniqueness IsUnique(string filename, int profileId)
        {
            var fileNameUniqueness = Uniqueness.NotUnique;
            var docs = _reader.GetDocuments(filename);

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