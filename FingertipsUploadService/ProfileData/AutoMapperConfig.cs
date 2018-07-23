using FingertipsUploadService.ProfileData.Entities.Core;
using AutoMapper;

namespace FingertipsUploadService.ProfileData
{
    public static class AutoMapperConfig
    {
        private static bool _isInitialized;

        public static void RegisterMappings()
        {
            if (_isInitialized == false)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<UploadDataModel, CoreDataSet>();
                    cfg.CreateMap<CoreDataSet, CoreDataSetArchive>();
                    cfg.CreateMap<CoreDataSetArchive, CoreDataSet>();
                });
                Mapper.Configuration.CompileMappings();
                _isInitialized = true;
            }
        }

        public static CoreDataSet ToCoreDataSet(this UploadDataModel uploadDataModel)
        {
            var coreDataSet = new CoreDataSet();
            Mapper.Map(uploadDataModel, coreDataSet);
            return coreDataSet;
        }

        public static CoreDataSetArchive ToCoreDataSetArchive(this CoreDataSet coreDataSet)
        {
            var coreDataSetArchive = new CoreDataSetArchive();
            Mapper.Map(coreDataSet, coreDataSetArchive);
            return coreDataSetArchive;

        }
    }
}