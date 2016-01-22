using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profiles.DomainObjects
{
    /// <summary>
    /// Site skin.
    /// </summary>
    public class Skin
    {
        public int Id { get; set; }
        public string PartialViewFolder { get; set; }
        public string Name { get; set; }
        public string MetaDescription { get; set; }
        public string TemplateProfileUrlKey { get; set; }
        public string Host { get; set; }
        public string GoogleAnalyticsKey { get; set; }
        public string Title { get; set; }
        public string TestHost { get; set; }
        public string LiveHost { get; set; }
        public string AccessControlGroup { get; set; }

        public bool IsTitle
        {
            get
            {
                return string.IsNullOrWhiteSpace(Title) == false;
            }
        }

        public bool IsLongerLives
        {
            get
            {
                return Id == SkinIds.LongerLives;
            }
        }

        public bool IsPhof
        {
            get
            {
                return Id == SkinIds.Phof;
            }
        }

        public bool IsTobacco
        {
            get
            {
                return Id == SkinIds.Tobacco;                
            }
        }
    }
}
