using System;
using System.Web;
using Profiles.DataAccess;
using Profiles.DomainObjects;

namespace Profiles.MainUI.Skins
{
    public static class SkinFactory
    {
        public static Skin GetSkin()
        {
            var appSettings = new AppConfig(AppConfig.AppSettings);
            var hostName = HttpContext.Current.Request.Url.Host;
            var reader = ReaderFactory.GetProfileReader();

            Skin skin = appSettings.IsSkinOverride ?
                reader.GetSkinFromName(appSettings.SkinOverride) :
                reader.GetSkin(appSettings.Environment, hostName);

            if (skin == null)
            {
                var message = string.IsNullOrWhiteSpace(appSettings.SkinOverride) ?
                            "SkinOverride is not set in Web.Config and skin cannot be identified from the URL host."            
                            : string.Format("SkinOverride {0} is not valid.  Please set a correct value in Web.Config", appSettings.SkinOverride) ;
        
                HttpContext.Current.Response.ClearContent();    // to handle any chain of calls
                HttpContext.Current.Response.Write(message);
                throw new ApplicationException(message);
            }
            
            skin.Host = hostName;
            return skin;
        }

        public static Skin NullSkin()
        {
            return new Skin
                {
                    MetaDescription = string.Empty
                };
        }
    }
}