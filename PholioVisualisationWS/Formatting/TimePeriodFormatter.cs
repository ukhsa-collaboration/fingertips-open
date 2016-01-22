
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Formatting
{
    public abstract class TimePeriodFormatter
    {
        public abstract void Format(Grouping grouping, IndicatorMetadata indicatorMetadata);

        public static string GetTimePeriodString(TimePeriod period, int yearTypeId)
        {
            // If need to format Baseline as well then pass in ad hoc 3 prop object

            if (period.IsQuarterly)
            {
                return FormatQuarter(period, yearTypeId);
            }
            if (period.IsMonthly)
            {
                return FormatMonth(period, yearTypeId);
            }

            return FormatYearly(period, yearTypeId);
        }

        private static string GetYearlyRangeByMonths(string start, string end, int year, int yearRange)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(start);
            sb.Append(" ");
            sb.Append(year);
            sb.Append(" - ");
            sb.Append(end);
            sb.Append(" ");
            sb.Append(year + yearRange);
            return sb.ToString();
        }

        private static string FormatYearly(TimePeriod period, int yearTypeId)
        {
            int yearRange = period.YearRange;
            int year = period.Year;

            string yearText = null;

            switch (yearTypeId)
            {
                case YearTypeIds.AugustToJuly:
                    yearText = GetYearlyRangeByMonths("Aug", "Jul", year, yearRange);
                    break;

                case YearTypeIds.MarchToFebruary:
                    yearText = GetYearlyRangeByMonths("Mar", "Feb", year, yearRange);
                    break;

                default:
                    // Year starts with year (1-3)
                    StringBuilder sb = new StringBuilder();
                    sb.Append(year);
                    if (yearRange == 1)
                    {
                        if (DoesYearNotBeginInJanuary(yearTypeId))
                        {
                            sb.Append("/");
                            sb.Append(GetLastTwoDigits(year + 1));
                        }
                    }
                    else
                    {
                        if (DoesYearNotBeginInJanuary(yearTypeId))
                        {
                            sb.Append("/");
                            sb.Append((year + 1).ToString().Substring(2, 2));
                            sb.Append(" - ");
                            sb.Append(GetLastTwoDigits(year + yearRange - 1));
                            sb.Append("/");
                            sb.Append(GetLastTwoDigits(year + yearRange));
                        }
                        else
                        {
                            sb.Append(" - ");
                            sb.Append(GetLastTwoDigits(year + yearRange - 1));
                        }
                    }
                    yearText = sb.ToString();
                    break;
            }

            return yearText;
        }

        private static bool DoesYearNotBeginInJanuary(int yearTypeId)
        {
            return yearTypeId == YearTypeIds.Financial ||
                yearTypeId == YearTypeIds.Academic;
        }

        private static string FormatQuarter(TimePeriod period, int yearTypeId)
        {
            StringBuilder sb = new StringBuilder();
            TimePoint point;

            switch (yearTypeId)
            {
                case YearTypeIds.FinancialRollingYearQuarterly:
                    AddNonCalendarYearQuarter(sb, period);
                    sb.Append(" - ");
                    AddNonCalendarYearQuarter(sb, period.GetRollingQuarterlyEndPoint());
                    break;

                case YearTypeIds.CalendarRollingYearQuarterly:
                    AddCalendarYearQuarter(sb, period);
                    sb.Append(" - ");
                    AddCalendarYearQuarter(sb, period.GetRollingQuarterlyEndPoint());
                    break;

                case YearTypeIds.FinancialSingleYearCumulativeQuarter:
                    point = new TimePoint { Year = period.Year, Quarter = 1 };
                    AddNonCalendarYearQuarter(sb, point);
                    if (period.Quarter > 1)
                    {
                        sb.Append("-Q");
                        sb.Append(period.Quarter);
                    }
                    break;

                case YearTypeIds.FinancialMultiYearCumulativeQuarter:

                    // Starting quarter
                    point = new TimePoint { Year = period.Year, Quarter = 1 };
                    AddNonCalendarYearQuarter(sb, point);

                    // Final quarter

                    if (period.Quarter > 1)
                    {
                        sb.Append(" - ");

                        int yearDifference = (int)Math.Ceiling(period.Quarter / 4.0) - 1;
                        point = new TimePoint { 
                            Year = point.Year + yearDifference,
                            Quarter = EnsureQuarterIsFourOrLess(period.Quarter)
                        };
                        AddNonCalendarYearQuarter(sb, point);
                    }
                    break;


                default:
                    if (DoesYearNotBeginInJanuary(yearTypeId))
                    {
                        AddNonCalendarYearQuarter(sb, period);
                    }
                    else
                    {
                        AddCalendarYearQuarter(sb, period);
                    }

                    break;
            }

            return sb.ToString();
        }

        private static int EnsureQuarterIsFourOrLess(int quarter)
        {
            while (quarter > 4)
            {
                quarter -= 4;
            }
            return quarter;
        }

        private static void AddNonCalendarYearQuarter(StringBuilder sb, TimePoint point)
        {
            sb.Append(point.Year);
            sb.Append("/");
            sb.Append(GetLastTwoDigits(point.Year + 1));
            sb.Append(" Q");
            sb.Append(point.Quarter);
        }

        private static void AddCalendarYearQuarter(StringBuilder sb, TimePoint point)
        {
            sb.Append(point.Year);
            sb.Append(" Q");
            sb.Append(point.Quarter);
        }

        private static string FormatMonth(TimePeriod period, int yearTypeId)
        {
            StringBuilder sb = new StringBuilder();

            switch (yearTypeId)
            {
                case YearTypeIds.CalendarRollingYearMonthly:
                    // Calendar rolling year monthly
                    AddMonthAndYear(period, sb);
                    sb.Append(" - ");
                    AddMonthAndYear(period.GetRollingMonthlyEndPoint(), sb);
                    break;

                default:
                    int month = period.Month;
                    int year = period.Year;
                    if (DoesYearNotBeginInJanuary(yearTypeId))
                    {
                        month += 3;
                        if (month > 12)
                        {
                            month -= 12;
                            year++;
                        }
                    }

                    AddMonthAndYear(new TimePoint { Year = year, Month = month }, sb);
                    break;
            }
            return sb.ToString();
        }

        private static void AddMonthAndYear(TimePoint point, StringBuilder sb)
        {
            int month = point.Month;
            if (month > 0 && month < 13)
            {
                sb.Append(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(point.Month).Substring(0, 3));
            }
            else
            {
                sb.Append(month);
            }
            sb.Append(" ");
            sb.Append(point.Year);
        }

        private static string GetLastTwoDigits(int year)
        {
            return year.ToString(CultureInfo.CurrentCulture).Substring(2, 2);
        }
    }
}
