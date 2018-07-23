
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PholioVisualisation.DataAccess;
using SpreadsheetGear;

namespace PholioVisualisation.Export.File
{
    public class ProfileFileInfo : BaseExcelFileInfo
    {
        private IEnumerable<string> parentAreaCodes;
        private string areaString;
        private int childAreaTypeId;
        private int parentAreaTypeId;
        private int profileId;

        public ProfileFileInfo(int profileId, IEnumerable<string> parentAreaCodes, 
            int childAreaTypeId, int parentAreaTypeId)
        {
            this.parentAreaCodes = parentAreaCodes;
            this.childAreaTypeId = childAreaTypeId;
            this.parentAreaTypeId = parentAreaTypeId;
            this.profileId = profileId;

            InitAreaString();
            FilePath = Path.Combine(ApplicationConfiguration.Instance.ExportFileDirectory, FileName);
        }

        /// <summary>
        /// The SpreadSheet Gear file format.
        /// </summary>
        public override FileFormat FileFormat
        {
            get
            {
                return FileFormat.OpenXMLWorkbook;
            }
        }

        public override string FileExtension
        {
            get
            {
                return "xlsx";
            }
        }

        public string FileName
        {
            get
            {
                return string.Format("{0}{1}{2}-{3}.{4}", profileId, areaString, 
                    childAreaTypeId, parentAreaTypeId, FileExtension);
            }
        }

        private void InitAreaString()
        {
            List<string> codes = parentAreaCodes.ToList();
            codes.Sort();

            StringBuilder sb = new StringBuilder();
            foreach (string code in codes)
            {
                sb.Append("-");
                sb.Append(code);
            }
            sb.Append("-");
            areaString = sb.ToString();
        }

    }
}
