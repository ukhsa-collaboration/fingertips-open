using AutoMapper;
using Fpm.MainUI.ViewModels;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProfileDetails, ProfileViewModel>();
                cfg.CreateMap<ProfileViewModel, ProfileDetails>();
                cfg.CreateMap<CoreDataSet, CoreDataSetArchive>();
            });
            Mapper.Configuration.CompileMappings();
        }
    }
}