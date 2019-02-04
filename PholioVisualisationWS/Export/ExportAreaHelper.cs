using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.Export
{
    public class ExportAreaHelper
    {
        // Dependencies
        private readonly IAreasReader _areasReader;
        private readonly IndicatorExportParameters _parameters;
        private readonly AreaFactory _areaFactory;

        // Locals
        private string[] _childAreaCodes;
        private Dictionary<string, List<string>> _parentAreaCodeToChildAreaCodesMap;
        private string[] _parentAreaCodes;
        private Dictionary<string, Area> _childAreaCodeToParentAreaMap;

        // Categories Enum
        public enum GeographicalCategory { National, SubNational, Local };

        public ExportAreaHelper(IAreasReader areasReader, IndicatorExportParameters parameters,
            AreaFactory areaFactory)
        {
            _areasReader = areasReader;
            _parameters = parameters;
            _areaFactory = areaFactory;
        }

        /// <summary>
        /// Init method to allow for lazy initialisation.
        /// </summary>
        public void Init()
        {
            if (_parentAreaCodes == null)
            {
                var parentAreas = GetParentAreas(_parameters);
                _parentAreaCodes = parentAreas.Select(x => x.Code).ToArray();

                // Init child to parent area map
                _childAreaCodeToParentAreaMap = _areasReader.GetParentAreasFromChildAreaId(_parameters.ParentAreaTypeId,
                    _parameters.ChildAreaTypeId);

                InitParentToChildAreaCodeMap();

                InitChildAreaCodes();

                // Populate look ups for area lists
                if (_parentAreaCodes.Length == 1 && Area.IsAreaListAreaCode(parentAreas.First().Code))
                {
                    InitChildCodeToParentAreaMapForAreaList(parentAreas);
                }
            }
        }

        private void InitChildCodeToParentAreaMapForAreaList(IList<IArea> parentAreas)
        {
            var parentArea = parentAreas.First();
            foreach (var childAreaCode in _childAreaCodes)
            {
                _childAreaCodeToParentAreaMap.Add(childAreaCode, (Area) parentArea);
            }
        }

        public string[] ParentAreaCodes
        {
            get { return _parentAreaCodes; }
        }

        public string[] ChildAreaCodes
        {
            get
            {
                return _childAreaCodes;
            }
        }

        /// <summary>
        /// Dictionary where key is parent area code, value is list of child area codes
        /// </summary>
        public Dictionary<string, List<string>> ParentAreaCodeToChildAreaCodesMap
        {
            get
            {
                return _parentAreaCodeToChildAreaCodesMap;
            }
        }

        /// <summary>
        /// Dictionary where key is child area code, value its parent area
        /// </summary>
        public Dictionary<string, Area> ChildAreaCodeToParentAreaMap
        {
            get
            {
                return _childAreaCodeToParentAreaMap;
            }
        }

        public IArea England
        {
            get { return _areaFactory.NewArea(AreaCodes.England); }
        }

        private void InitParentToChildAreaCodeMap()
        {
            _parentAreaCodeToChildAreaCodesMap = new Dictionary<string, List<string>>();
            foreach (var parentAreaCode in _parentAreaCodes)
            {
                var areaCodes = new List<string>();
                _parentAreaCodeToChildAreaCodesMap.Add(parentAreaCode, areaCodes);
                foreach (var kvp in _childAreaCodeToParentAreaMap)
                {
                    if (kvp.Value.Code == parentAreaCode)
                    {
                        // Add child area code
                        areaCodes.Add(kvp.Key);
                    }
                }
            }
        }

        private void InitChildAreaCodes()
        {
            var childAreas = new FilteredChildAreaListProvider(_areasReader)
                .ReadChildAreas(_parameters.ParentAreaCode, _parameters.ProfileId, _parameters.ChildAreaTypeId);
            _childAreaCodes = childAreas.Select(x => x.Code).ToArray();
        }

        private IList<IArea> GetParentAreas(IndicatorExportParameters parameters)
        {
            IList<IArea> parentAreas;
            if (parameters.ParentAreaCode.Equals(AreaCodes.England,
                StringComparison.CurrentCultureIgnoreCase))
            {
                // all for parent area type
                parentAreas = new ChildAreaListBuilder(_areasReader).GetChildAreas(parameters.ParentAreaCode,
                    parameters.ParentAreaTypeId);
            }
            else
            {
                // one parent
                var parent = _areaFactory.NewArea(parameters.ParentAreaCode);
                parentAreas = new List<IArea> { parent };
            }
            return parentAreas;
        }

        public static GeographicalCategory GetGeographicalCategory(IArea parentArea = null)
        {
            if (parentArea == null)
                return GeographicalCategory.National;

            return parentArea.Code.Equals(AreaCodes.England) ? GeographicalCategory.SubNational : GeographicalCategory.Local;
        }
    }
}