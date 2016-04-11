using Fpm.MainUI.ViewModels;
using Fpm.ProfileData.Entities.Core;
using Fpm.ProfileData.Entities.Profile;

namespace Fpm.MainUI
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<ProfileDetails, ProfileViewModel>();
            AutoMapper.Mapper.CreateMap<ProfileViewModel, ProfileDetails>();
            AutoMapper.Mapper.CreateMap<CoreDataSet, CoreDataSetArchive>();

        }
    }
}