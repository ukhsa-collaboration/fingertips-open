using AutoMapper;
using Fpm.MainUI.Helpers;
using Fpm.MainUI.ViewModels.Profile;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            // To use =? AutoMapper.Mapper.Map(profileDetails, profileViewModel);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProfileDetails, ProfileViewModel>();
                cfg.CreateMap<ProfileViewModel, ProfileDetails>();
                cfg.CreateMap<ProfileViewModel, ExtraJsFileHelper>();
                cfg.CreateMap<ExtraJsFileHelper, ProfileViewModel>();
                cfg.CreateMap<AreaType, AreaType>();
                cfg.CreateMap<CoreDataSet, CoreDataSetArchive>();
                cfg.CreateMap<Grouping, Grouping>();
                cfg.CreateMap<ContentItem, ContentItem>();
            });
            Mapper.Configuration.CompileMappings();
        }
    }
}