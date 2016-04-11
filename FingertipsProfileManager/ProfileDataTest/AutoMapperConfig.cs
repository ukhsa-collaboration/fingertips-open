using Fpm.ProfileData.Entities.Core;

namespace Fpm.ProfileDataTest
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<CoreDataSet, CoreDataSetArchive>();
        }
    }
}