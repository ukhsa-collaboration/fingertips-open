
using FingertipsUploadService.ProfileData.Entities.Core;

namespace FingertipsUploadServiceTest
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            AutoMapper.Mapper.CreateMap<CoreDataSet, CoreDataSetArchive>();
        }
    }
}
