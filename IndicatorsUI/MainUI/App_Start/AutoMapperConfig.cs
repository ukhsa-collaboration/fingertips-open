using AutoMapper;
using IndicatorsUI.MainUI.Configuration;

namespace IndicatorsUI.MainUI
{
    public class AutoMapperConfig
    {
        private static bool _hasBeenInitialized;

        public static void RegisterMappings()
        {
            if (_hasBeenInitialized == false)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile(new IndicatorListMappingProfile());
                });
                Mapper.Configuration.CompileMappings();

                _hasBeenInitialized = true;
            }
        }
    }


}