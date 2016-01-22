using System.Collections.Generic;
using System.IO;
using System.Linq;
using PholioVisualisation.DataAccess;

namespace PholioVisualisation.Export
{
    public class PracticeProfileFileInfo : BaseExcelFileInfo
    {
        private string profileKey;
        private IList<int> groupIds;
        private string areaCode;

        public PracticeProfileFileInfo(string profileKey, IList<int> groupIds, string areaCode)
        {
            this.profileKey = profileKey;
            this.groupIds = groupIds;
            this.areaCode = areaCode;
            FilePath = Path.Combine(ApplicationConfiguration.ExportFileDirectory, FileName);
        }

        public string FileName
        {
            get
            {
                // risky way to create unique id
                string groupIdsString = string.Join("-",groupIds);

                return string.Format("{0}-{1}-{2}.{3}", profileKey, areaCode, groupIdsString, FileExtension);
            }
        }

    }
}
