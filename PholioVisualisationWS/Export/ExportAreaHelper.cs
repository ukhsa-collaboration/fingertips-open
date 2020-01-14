using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.Export.FileBuilder.Wrappers;

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
        private Dictionary<string, List<string>> _parentCategoryAreaIdToAreaCodesMap;
        private IList<IArea> _parentAreas;

        private readonly int _categoryTypeId;
        private const string CategoryStartingName = "cat-";

        // Categories Enum
        public enum GeographicalCategory { National, SubNational, Local };

        public ExportAreaHelper(IAreasReader areasReader, IndicatorExportParameters parameters)
        {
            _areasReader = areasReader;
            _parameters = parameters;
            _areaFactory = new AreaFactory(areasReader);

            _categoryTypeId = CategoryTypeIds.Undefined;

            if (IsCategoryAreaTypeId(parameters.ParentAreaTypeId))
            {
                _categoryTypeId = GetCategoryTypeId(parameters.ProfileId, parameters.ParentAreaTypeId);
            }
        }

        /// <summary>
        /// Init method to allow for lazy initialisation.
        /// </summary>
        public void Init()
        {
            if (_parentAreaCodes != null) return;

            InitParentAreasCodes();

            InitChildAreaCodeToParentAreaMap();

            InitParentToChildAreaCodeMap();

            InitChildAreaCodes();

            InitChildCodeToParentAreaMapForAreaList();

            InitParentCategoryAreaIdToAreaCodesMap();
        }

        public string[] GetAreaCodes()
        {
            var areaCodes = ParentAreaCodes;

            if (areaCodes.Length == 0)
            {
                return areaCodes;
            }

            // Get the the list of areaCodes if we received a publicId
            if (!IsAreaListCode(areaCodes[0]) && !IsNearestNeighboursCode(areaCodes[0]))
            {
                return areaCodes;
            }

            areaCodes = ChildAreaCodes;
            return areaCodes;
        }

        public string GetPopulationAreaCode(IArea parentArea, OnDemandQueryParametersWrapper onDemandQueryParameters, string childAreaCode)
        {
            if (IsAreaListCode(parentArea.Code))
            {
                return parentArea.Code;
            }

            var areaCodes = onDemandQueryParameters.ChildAreaCodeList;
            if (areaCodes != null && IsNearestNeighboursCode(areaCodes[0]))
            {
                return areaCodes[0];
            }

            return IsCategoryParentAreaCode() ? onDemandQueryParameters.CategoryAreaCode[0] : childAreaCode;
        }

        public int GetPopulationAreaTypeId(IArea parentArea, OnDemandQueryParametersWrapper onDemandQueryParameters)
        {
            var areaTypeId = parentArea.AreaTypeId;

            if (IsAreaListCode(parentArea.Code)) return areaTypeId;

            var areaCodes = onDemandQueryParameters.ChildAreaCodeList;
            var isNN = areaCodes != null && IsNearestNeighboursCode(areaCodes[0]);

            return isNN ? areaTypeId : _parameters.ChildAreaTypeId;
        }

        public IArea GetParentArea()
        {
            var parentArea = England;
            var areaCodes = ParentAreaCodes;

            if (areaCodes.Length == 0)
            {
                return parentArea;
            }

            // Get the list of areaCodes if we received a publicId
            if (!IsAreaListCode(areaCodes[0]) && !IsNearestNeighboursCode(areaCodes[0]))
            {
                return parentArea;
            }

            parentArea = _areaFactory.NewArea(ParentAreaCodes[0]);
            return parentArea;
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

        public int CategoryTypeId
        {
            get
            {
                return _categoryTypeId;
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


        public Dictionary<string, List<string>> ParentCategoryAreaIdToChildAreaCodesMap
        {
            get
            {
                return _parentCategoryAreaIdToAreaCodesMap;
            }
        }

        private IArea _england;
        public IArea England
        {
            get
            {
                if (_england == null)
                {
                    _england = _areaFactory.NewArea(AreaCodes.England);
                }

                return _england;
            }
        }

        private void InitParentAreasCodes()
        {
            _parentAreas = GetParentAreas();

            _parentAreaCodes = _parentAreas.Select(x => x.Code).ToArray();
        }

        // Init child to parent area map
        private void InitChildAreaCodeToParentAreaMap()
        {
            if (IsCategoryAreaTypeId(_parameters.ParentAreaTypeId))
            {
                var categoryTypeId = GetCategoryTypeId(_parameters.ProfileId, _parameters.ParentAreaTypeId);
                _childAreaCodeToParentAreaMap = _areasReader.GetParentAreasFromCategoryTypeIdAndChildAreaTypeId(categoryTypeId, _parameters.ChildAreaTypeId);
                _childAreaCodeToParentAreaMap.Add("E92000001", new Area { AreaTypeId = 15, Code = "E92000001", Name = "England", IsCurrent = true, ShortName = "England" });
            }
            else
            {
                _childAreaCodeToParentAreaMap = _areasReader.GetParentAreasFromChildAreaId(_parameters.ParentAreaTypeId, _parameters.ChildAreaTypeId);
            }
        }

        private void InitChildCodeToParentAreaMapForAreaList()
        {
            if (_parentAreas == null || _parentAreaCodes == null) throw new ArgumentNullException("Parent areas or parent areas code has to be initizalizate before calling this method");

            // Populate look ups for area lists
            if (_parentAreaCodes.Length != 1 || !Area.IsAreaListAreaCode(_parentAreas.First().Code)) return;

            var parentArea = _parentAreas.First();
            foreach (var childAreaCode in _childAreaCodes)
            {
                if (!_childAreaCodeToParentAreaMap.ContainsKey(childAreaCode))
                {
                    _childAreaCodeToParentAreaMap.Add(childAreaCode, (Area)parentArea);
                }
            }
        }

        private void InitParentToChildAreaCodeMap()
        {
            if (_parentAreaCodes == null) throw new ArgumentNullException("Parent areas code has to be initizalizate before calling this method");

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
            var childAreas = new FilteredChildAreaListProvider(_areasReader).ReadChildAreas(_parameters.ParentAreaCode, _parameters.ProfileId, _parameters.ChildAreaTypeId);
            _childAreaCodes = childAreas.Select(x => x.Code).ToArray();
        }

        private void InitParentCategoryAreaIdToAreaCodesMap()
        {
            _parentCategoryAreaIdToAreaCodesMap = new Dictionary<string, List<string>>();

            if (IsCategoryAreaTypeId(_parameters.ParentAreaTypeId))
            {
                _parentCategoryAreaIdToAreaCodesMap = GetParentAreasFromChildAreaId();
            }
        }

        private IList<IArea> GetParentAreas()
        {
            if (IsCategoryAreaTypeId(_parameters.ParentAreaTypeId))
            {
                return ParentAreaCodeFromListAreas(GetParentAreasCategoryAreaTypeId(_parameters.ProfileId, _parameters.ParentAreaTypeId));
            }

            if (IsEnglandParentAreaCode())
            {
                // all for parent area type
                return new ChildAreaListBuilder(_areasReader).GetChildAreas(_parameters.ParentAreaCode, _parameters.ParentAreaTypeId);
            }

            // one parent
            var parent = _areaFactory.NewArea(_parameters.ParentAreaCode);
            return new List<IArea> { parent };
        }

        private Dictionary<string, List<string>> GetParentAreasFromChildAreaId()
        {
            var categoryAreaIdToAreaCodesMap = new Dictionary<string, List<string>>();

            var parentAreas = GetParentAreasCategoryAreaTypeId(_parameters.ProfileId, _parameters.ParentAreaTypeId);

            foreach (var parentArea in parentAreas)
            {
                var parentChildAreaRelationship = new ParentChildAreaRelationshipBuilder(new IgnoredAreasFilter(new List<string>()), new AreaListProvider(_areasReader))
                    .GetParentAreaWithChildAreas(parentArea, _parameters.ChildAreaTypeId, false);
                var childrenAreaCodeList = parentChildAreaRelationship.Children.Select(x => x.Code).ToList();

                categoryAreaIdToAreaCodesMap.Add(parentChildAreaRelationship.Parent.Code, childrenAreaCodeList);
            }

            return categoryAreaIdToAreaCodesMap;
        }

        public static GeographicalCategory GetGeographicalCategory(IArea parentArea = null)
        {
            if (parentArea == null)
                return GeographicalCategory.National;

            return parentArea.Code.Equals(AreaCodes.England) ? GeographicalCategory.SubNational : GeographicalCategory.Local;
        }

        public static bool IsNearestNeighboursCode(string areaCode)
        {
            return Area.IsNearestNeighbour(areaCode);
        }

        public static bool IsAreaListCode(string parentAreaCode)
        {
            return Area.IsAreaListAreaCode(parentAreaCode);
        }

        public static bool IsCategoryAreaTypeId(int ParentAreaTypeId)
        {
            return ParentAreaTypeId >= CategoryAreaType.IdAddition && ParentAreaTypeId < AreaTypeIds.AreaList;
        }

        public bool IsEnglandParentAreaCode()
        {
            return _parameters.ParentAreaCode.Equals(AreaCodes.England, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsCategoryParentAreaCode()
        {
            return _parameters.ParentAreaCode.Contains(CategoryStartingName);
        }

        public string[] ParentAreaCodeFromCategoryTypeId(string[] categoriesTypeId)
        {
            var differentParentsAreasCode = new List<string>();
            foreach (var categoryTypeId in categoriesTypeId)
            {
                var parentAreaCode = ((ICategoryArea)_parentAreas.FirstOrDefault(areaType => areaType.Code == categoryTypeId)).ParentAreaCode;
                if (!differentParentsAreasCode.Contains(parentAreaCode))
                {
                    differentParentsAreasCode.Add(parentAreaCode);
                }
            }

            return differentParentsAreasCode.ToArray();
        }

        public List<IArea> ParentAreaCodeFromListAreas(IList<IArea> categoriesTypeIdAreaList)
        {
            var differentParentsAreasCode = new List<IArea>();
            foreach (var area in categoriesTypeIdAreaList)
            {
                var any = false;
                foreach (var diffArea in differentParentsAreasCode)
                {
                    if (diffArea.Code != ((ICategoryArea)area).ParentAreaCode) continue;
                    any = true;
                    break;
                }

                if (!any)
                {
                    differentParentsAreasCode.Add(AreaFactory.NewArea(_areasReader, ((ICategoryArea)area).ParentAreaCode));
                }
            }

            return differentParentsAreasCode;
        }

        public static bool IsCategoryAreaCode(string areaCode)
        {
            return areaCode.Contains(CategoryStartingName);
        }

        public static bool IsCategoryAreaCode(string[] areaCode)
        {
            return areaCode.Any(x => x.Contains(CategoryStartingName));
        }

        public static CategoryInequalitySearch GetCategoryInequalityFromAreaCode(string parentAreaCode, IAreasReader areasReader)
        {
            var area = AreaFactory.NewArea(areasReader, parentAreaCode) as CategoryArea;

            if (area == null)
            {
                throw new ArgumentException("The parent code is not a category id");
            }

            return new CategoryInequalitySearch(area.CategoryTypeId, area.CategoryId);
        }

        public static IList<IArea> GetParentAreasCategoryAreaTypeId(int profileId, int parentAreaTypeId)
        {
            var listBuilder = new AreaListProvider(ReaderFactory.GetAreasReader());
            listBuilder.CreateAreaListFromAreaTypeId(profileId, parentAreaTypeId, null);
            return listBuilder.Areas;
        }

        public static int GetCategoryTypeId(int profileId, int parentAreaTypeId)
        {
            var area = (ICategoryArea)GetParentAreasCategoryAreaTypeId(profileId, parentAreaTypeId).FirstOrDefault();
            if (area == null) return -1;

            return area.CategoryTypeId;
        }
    }
}