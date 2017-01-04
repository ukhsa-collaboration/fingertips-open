using AutoMapper;

namespace ServicesWeb
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                //  cfg.CreateMap<CoreDataSet, CoreDataSetArchive>();
            });
            Mapper.Configuration.CompileMappings();
        }
    }
}