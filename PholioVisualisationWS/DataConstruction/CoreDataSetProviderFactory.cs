using System;
using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class CoreDataSetProviderFactory
    {
        private CcgPopulationProvider ccgPopulationProvider;

        public CoreDataSetProvider New(IArea area)
        {
            Area _area =  area as Area;

            if (_area == null)
            {
                CategoryArea categoryArea = area as CategoryArea;
                if (categoryArea.IsGpDeprivationDecile)
                {
                    return new GpDeprivationDecileCoreDataSetProvider(categoryArea, PracticeDataAccess);
                }
                return new CategoryAreaCoreDataSetProvider(categoryArea, GroupDataReader);
            }

            // Area 
            if (_area.IsCcg)
            {
                return new CcgCoreDataSetProvider(_area,
                    CcgPopulationProvider, CoreDataSetListProvider, GroupDataReader);
            }

            if (_area.IsShape)
            {
                return new ShapeCoreDataSetProvider(_area, PracticeDataAccess);
            }

            return new SimpleCoreDataSetProvider(_area, GroupDataReader);
        }

        private static PracticeDataAccess PracticeDataAccess
        {
            get
            {
                return new PracticeDataAccess();
            }
        }

        private static CoreDataSetListProvider CoreDataSetListProvider
        {
            get
            {
                return new CoreDataSetListProvider(GroupDataReader);
            }
        }

        private static IGroupDataReader GroupDataReader
        {
            get
            {
                return ReaderFactory.GetGroupDataReader();
            }
        }

        /// <summary>
        /// Allows the same provider to be used in multiple providers.
        /// </summary>
        public CcgPopulationProvider CcgPopulationProvider
        {
            get
            {
                if (ccgPopulationProvider != null)
                {
                    return ccgPopulationProvider;
                }

                return ccgPopulationProvider = new CcgPopulationProvider(ReaderFactory.GetPholioReader());
            }
            set
            {
                ccgPopulationProvider = value;
            }
        }
    }
}