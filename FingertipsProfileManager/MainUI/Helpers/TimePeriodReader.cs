using Fpm.ProfileData;
using System;
using System.IO;
using System.Net;

namespace Fpm.MainUI.Helpers
{
    public class TimePeriodReader : ITimePeriodReader
    {
        private bool _isWebServiceAvailable = true;

        public string GetPeriodString(TimePeriod timePeriod, int yearTypeId)
        {
            if (_isWebServiceAvailable == false)
            {
                return "No Web Services";
            }

            string period;
            try
            {
                period = GetPeriodFromWebService(timePeriod, yearTypeId);
            }
            catch (Exception)
            {
                _isWebServiceAvailable = false;
                return "No web services";
            }

            return period;
        }

        public static string GetPeriodFromWebService(TimePeriod timePeriod, int yearTypeId)
        {
            string period;
            string pholioWs = AppConfig.GetPholioWs();
            var serviceUrl = pholioWs + "api/time_period?quarter=" + timePeriod.Quarter +
                             "&month=" + timePeriod.Month +
                             "&year=" + timePeriod.Year +
                             "&year_range=" + timePeriod.YearRange +
                             "&year_type_id=" + yearTypeId;

            var httpWReq = (HttpWebRequest) WebRequest.Create(serviceUrl);
            HttpWebResponse httpWResp;
            try
            {
                httpWResp = (HttpWebResponse) httpWReq.GetResponse();
            }
            catch (WebException ex)
            {
                throw new FpmException("PholioVisualisationWS not started", ex);
            }

            using (var rd = new StreamReader(httpWResp.GetResponseStream()))
            {
                period = rd.ReadToEnd();
            }

            return period.Replace("\"", "");
        }
    }
}