namespace Fpm.Search
{
    public class IndicatorSearchQueryText
    {
        public string GetSqlSearchText(string userText)
        {
            // Replace punctuation with single character SQL wild card
            userText = userText
                .Replace(" ", "_")
                .Replace("-", "_");

            return "%" + userText + "%";
        }
    }
}