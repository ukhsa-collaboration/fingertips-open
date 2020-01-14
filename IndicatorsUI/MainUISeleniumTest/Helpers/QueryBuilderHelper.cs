using System.Collections.Generic;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public static class QueryBuilderHelper
    {
        public static IDictionary<string, IEnumerable<object>> CreateDictionaryOfIdValue(string idName, IEnumerable<object> stringList)
        {
            return new Dictionary<string, IEnumerable<object>> { { idName, stringList } };
        }

        public static string BuildWhereCondition(IDictionary<string, IEnumerable<object>> dictionary)
        {
            var whereCondition = "WHERE ";

            var index = 0;
            foreach (var element in dictionary)
            {
                foreach (var item in element.Value)
                {
                    if (index >= 1)
                    {
                        whereCondition += " OR ";
                    }

                    whereCondition += element.Key + "=";

                    if (item is string)
                    {
                        whereCondition += "'" + item + "'";
                    }
                    else if (item is bool)
                    {
                        if ((bool)item)
                        {
                            whereCondition += "1";
                        }
                        else
                        {
                            whereCondition += "0";
                        }
                    }
                    else
                    {
                        whereCondition += item;
                    }

                    index++;
                }
            }

            return whereCondition;
        }

        public static string BuildValueCondition(IEnumerable<object> list)
        {
            var whereCondition = "VALUES(";

            var index = 0;
            foreach (var element in list)
            {
                if (index >= 1)
                {
                    whereCondition += ",";
                }

                if (element is string)
                {
                    whereCondition += "'" + element + "'";
                }
                else if (element is bool)
                {
                    if ((bool)element)
                    {
                        whereCondition += "1";
                    }
                    else
                    {
                        whereCondition += "0";
                    }
                }
                else
                {
                    whereCondition += element;
                }
                index++;
            }

            whereCondition += ")";

            return whereCondition;
        }
    }
}
