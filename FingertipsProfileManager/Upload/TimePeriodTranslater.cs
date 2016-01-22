using Fpm.ProfileData;

namespace Fpm.Upload
{
    public class TimePeriodTranslater
    {
        public string Translate(SimpleUpload simpleUpload)
        {
            var period = new TimePeriod
            {
                Year = simpleUpload.Year,
                YearRange = simpleUpload.YearRange,
                Quarter = simpleUpload.Quarter,
                Month = simpleUpload.Month
            };

            string periodString = TimePeriodReader.GetPeriodFromWebService(period,
                simpleUpload.YearTypeId);

            return TranslateMonth(simpleUpload) + " " + TranslateQuarter(simpleUpload) +
                   " For Range: " + periodString;
        }

        private static string TranslateQuarter(SimpleUpload simpleUpload)
        {
            switch (simpleUpload.YearTypeId)
            {
                case 1: //Calendar
                    switch (simpleUpload.Quarter)
                    {
                        case -1:
                            return null;
                        case 1:
                            return "January to March - " + simpleUpload.Year;
                        case 2:
                            return "April to June - " + simpleUpload.Year;
                        case 3:
                            return "July to September - " + simpleUpload.Year;
                        case 4:
                            return "October to December - " + simpleUpload.Year;
                    }
                    break;
                case 2: //Financial
                    switch (simpleUpload.Quarter)
                    {
                        case -1:
                            return null;
                        case 1:
                            return "April to June - " + simpleUpload.Year;
                        case 2:
                            return "July to September - " + simpleUpload.Year;
                        case 3:
                            return "October to December - " + simpleUpload.Year;
                        case 4:
                            return "January to March - " + simpleUpload.Year++;
                    }
                    break;
            }

            return null;
        }

        private static string TranslateMonth(SimpleUpload simpleUpload)
        {
            switch (simpleUpload.YearTypeId)
            {
                case 1: //Calendar
                    switch (simpleUpload.Month)
                    {
                        case -1:
                            return null;
                        case 1:
                            return "January " + simpleUpload.Year;
                        case 2:
                            return "February " + simpleUpload.Year;
                        case 3:
                            return "March " + simpleUpload.Year;
                        case 4:
                            return "April " + simpleUpload.Year;
                        case 5:
                            return "May " + simpleUpload.Year;
                        case 6:
                            return "June " + simpleUpload.Year;
                        case 7:
                            return "July " + simpleUpload.Year;
                        case 8:
                            return "August " + simpleUpload.Year;
                        case 9:
                            return "September " + simpleUpload.Year;
                        case 10:
                            return "October " + simpleUpload.Year;
                        case 11:
                            return "November " + simpleUpload.Year;
                        case 12:
                            return "December " + simpleUpload.Year;
                    }
                    break;
                case 2: //Financial
                    switch (simpleUpload.Month)
                    {
                        case -1:
                            return null;
                        case 1:
                            simpleUpload.Month = 10;
                            return " January - " + simpleUpload.Year;
                        case 2:
                            simpleUpload.Month = 11;
                            return " February - " + simpleUpload.Year;
                        case 3:
                            simpleUpload.Month = 12;
                            return " March - " + simpleUpload.Year;
                        case 4:
                            simpleUpload.Month = 1;
                            return " April - " + simpleUpload.Year;
                        case 5:
                            simpleUpload.Month = 2;
                            return " May - " + simpleUpload.Year;
                        case 6:
                            simpleUpload.Month = 3;
                            return " June - " + simpleUpload.Year;
                        case 7:
                            simpleUpload.Month = 4;
                            return " July - " + simpleUpload.Year;
                        case 8:
                            simpleUpload.Month = 5;
                            return " August - " + simpleUpload.Year;
                        case 9:
                            simpleUpload.Month = 6;
                            return " September - " + simpleUpload.Year;
                        case 10:
                            simpleUpload.Month = 7;
                            return " October - " + simpleUpload.Year++;
                        case 11:
                            simpleUpload.Month = 8;
                            return " November - " + simpleUpload.Year++;
                        case 12:
                            simpleUpload.Month = 9;
                            return " December - " + simpleUpload.Year++;
                    }
                    break;
            }

            return null;
        }
    }
}