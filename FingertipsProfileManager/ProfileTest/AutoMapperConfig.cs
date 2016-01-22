using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Core;

namespace ProfileDataTest
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<UploadDataModel, CoreDataSet>();
            AutoMapper.Mapper.CreateMap<CoreDataSet, CoreDataSetArchive>();
        }
    }
}