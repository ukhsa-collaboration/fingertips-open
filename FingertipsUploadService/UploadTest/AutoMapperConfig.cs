
using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Entities.Core;

namespace FingertipsUploadService.UploadTest
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