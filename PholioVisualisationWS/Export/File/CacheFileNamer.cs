using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using PholioVisualisation.Export.FileBuilder.Containers;

namespace PholioVisualisation.Export.File
{
    public class CacheFileNamer
    {
        private static readonly char[] Padding = { '=' };

        private const int MaximumFileNameLength = 194/*MaximumCharFileNameLengthInWindows*/ - 6/*indicator Id plus hyphen*/;

        public static string GetIndicatorFileName(int indicatorId, CsvBuilderAttributesForBodyContainer parameters)
        {
            var parametersInOneString = NoneCompressedJoinedString(indicatorId, parameters);

            var compressedString = CompressString(parametersInOneString);

            var compressedFileName = string.Concat(compressedString, ".data.csv");

            var fileName = compressedFileName.Length <= MaximumFileNameLength ? compressedFileName : TruncateFileName(compressedFileName);

            // Prepend indicator ID so that cached files can be easily deleted on disc if required
            return indicatorId + "-" + fileName;
        }

        public static string GetAddressFileName(string parentAreaCode, int areaTypeId)
        {
            return string.Format("{0}-{1}.addresses.csv",
                parentAreaCode, areaTypeId);
        }

        public static string NoneCompressedJoinedString(int indicatorId, CsvBuilderAttributesForBodyContainer parameters)
        {
            var profileId = parameters.GeneralParameters.ProfileId;
            var parentAreaCode = parameters.GeneralParameters.ParentAreaCode;
            var parentAreaTypeId = parameters.GeneralParameters.ParentAreaTypeId;
            var childAreaTypeId = parameters.GeneralParameters.ChildAreaTypeId;

            var allPeriods = parameters.OnDemandParameters.AllPeriods;
            var childAreaCodeList = parameters.OnDemandParameters.ChildAreaCodeList;
            var indicatorsIdsList = parameters.OnDemandParameters.IndicatorIds;
            var groupIds = parameters.OnDemandParameters.GroupIds;

            var stringChildAreaCodeList = childAreaCodeList != null ? string.Join("", childAreaCodeList.ToArray()) : "All_Areas";
            var stringIndicatorIdsList = indicatorsIdsList != null ? string.Join("", indicatorsIdsList.ToArray()) : "";
            var stringGroupIdsList = indicatorsIdsList != null ? string.Join("", groupIds.ToArray()) : "";

            // Do not include indicator ID because it will be added to the start of the filename
            var parametersJoinedString = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                profileId, parentAreaCode, parentAreaTypeId, childAreaTypeId, allPeriods, stringChildAreaCodeList,
                stringIndicatorIdsList, stringGroupIdsList);

            return parametersJoinedString;
        }

        public static string CompressString(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

            return Convert.ToBase64String(gZipBuffer).TrimEnd(Padding).Replace('+', '-').Replace('/', '_');
        }

        public static string TruncateFileName(string compressedFileName)
        {
            var startIndex = compressedFileName.Length - (compressedFileName.Length - (compressedFileName.Length - MaximumFileNameLength));

            var truncateCompressedSubstring = compressedFileName.Substring(startIndex);

            return truncateCompressedSubstring;
        }
    }
}