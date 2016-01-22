using Profiles.DomainObjects;

namespace Profiles.MainUI.Models
{
    public class ContentTab
    {
        public ContentTab(PageModel model)
        {
            if (model.Skin.IsPhof)
            {
                ContentKey = ContentKeys.Inequalities;
            }
        }

        public string ContentKey { get; set; }
    }
}