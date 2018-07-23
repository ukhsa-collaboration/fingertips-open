using NHibernate;
using NHibernate.Criterion;
using IndicatorsUI.DomainObjects;

namespace IndicatorsUI.DataAccess
{
    public class TwitterSettingReader : BaseReader
    {
        internal TwitterSettingReader(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public TwitterAccountSetting GetSettings(string handle)
        {
            var twitterSettings = CurrentSession.CreateCriteria<TwitterAccountSetting>()
                .Add(Restrictions.Eq("Handle", handle))
                .UniqueResult<TwitterAccountSetting>();
            return twitterSettings;           
        }
    }
}
