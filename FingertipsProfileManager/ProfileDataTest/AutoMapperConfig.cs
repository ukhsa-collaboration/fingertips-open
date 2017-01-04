using AutoMapper;
using Fpm.ProfileData.Entities.Core;

namespace Fpm.ProfileDataTest
{
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Register mappings that only apply to ProfileData
        /// </summary>
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CoreDataSet, CoreDataSetArchive>();
            });
            Mapper.Configuration.CompileMappings();
        }
    }
}