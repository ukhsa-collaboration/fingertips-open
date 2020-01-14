using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.DataAccess
{
    public interface IAreasReader
    {
        IList<ParentAreaGroup> GetParentAreaGroupsForProfile(int profileId);
        IList<ParentAreaGroup> GetParentAreaGroupsForChildAreaType(int childAreaTypeId);
        IList<ParentAreaGroup> GetParentAreaGroups(int profileId, int childAreaTypeId);
        Area GetAreaFromCode(string code);
        IList<IArea> GetAreasFromCodes(IEnumerable<string> codes);
        IList<IArea> GetAreasByAreaTypeId(int areaTypeId);
        IList<IArea> GetAreasByAreaTypeIdAndAreaNameSearchText(int areaTypeId, string areaNameSearchText);
        AreaAddress GetAreaWithAddressFromCode(string code);

        /// <summary>
        ///     Gets a URL to the web site of the area, e.g. a local authority home page.
        /// </summary>
        string GetAreaUrl(string areaCode);

        IList<PlaceName> GetAllPlaceNames();
        IList<PostcodeParentAreas> GetAllPostCodeParentAreasStartingWithLetter(string firstLetter);
        IList<AreaAddress> GetAreaWithAddressFromCodes(IEnumerable<string> codes);
        IList<AreaAddress> GetAreaWithAddressByAreaTypeId(int areaTypeId);
        IList<IArea> GetChildAreas(string parentArea, int childAreaTypeId);

        /// <summary>
        ///     Hash key(childAreaCode) -> value(parentArea)
        /// </summary>
        /// <param name="parentAreaTypeIds"></param>
        /// <param name="childAreaTypeId"></param>
        /// <returns></returns>
        Dictionary<string, Area> GetParentsFromChildAreaIdAndParentAreaTypeId(int parentAreaTypeIds, int childAreaTypeId);

        Dictionary<string, Area> GetParentAreasFromChildAreaId(int parentAreaTypeId, int childAreaTypeId);
        IList<string> GetParentCodesFromChildAreaId(int childAreaTypeId);
        IList<Area> GetParentAreas(string childAreaCode);
        IList<AreaType> GetAreaTypes(IEnumerable<int> areaTypeIds);
        IList<AreaType> GetAllAreaTypes();
        IList<AreaType> GetSupportedAreaTypes();
        IList<int> GetAllAreaTypeIds();
        AreaType GetAreaType(int areaTypeId);

        /// <summary>
        ///     Total number of areas of a given area type.
        /// </summary>
        int GetAreaCountForAreaType(int areaTypeId);

        int GetChildAreaCount(string parentAreaCode, int areaTypeId);
        int GetChildAreaCount(CategoryArea categoryArea, int childAreaTypeId);

        IList<string> GetAreaCodesThatDoNotExist(IList<string> areaCodes);
        IList<string> GetAreaCodesForAreaType(int areaTypeId);
        IList<string> GetChildAreaCodes(string parentAreaCode, int areaTypeId);

        IList<CategorisedArea> GetCategorisedAreasForAllCategories(int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId);

        IList<CategorisedArea> GetCategorisedAreasForOneCategory(int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId, int categoryId);

        CategorisedArea GetCategorisedArea(string areaCode, int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId);

        Category GetCategory(int categoryTypeId, int categoryId);
        IList<Category> GetCategories(int categoryTypeId);
        CategoryType GetCategoryType(int categoryTypeId);
        IList<CategoryType> GetCategoryTypes(IList<int> categoryTypeIds);
        IList<CategoryType> GetAllCategoryTypes();

        IList<NearByAreas> GetNearbyAreas(string easting, string northing, int areaTypeId);
        IList<AreaCodeNeighbourMapping> GetNearestNeighbours(string areaCode, int neighbourTypeId);

        Dictionary<string, Area> GetParentAreasFromCategoryTypeIdAndChildAreaTypeId(int parametersParentAreaTypeId, int parametersChildAreaTypeId);
        /// <summary>
        ///     Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        ///     IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }

    public class AreasReader : BaseReader, IAreasReader
    {
        /// <summary>
        ///     Parameterless constructor to allow mocking
        /// </summary>
        public AreasReader()
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        public AreasReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public virtual IList<ParentAreaGroup> GetParentAreaGroupsForProfile(int profileId)
        {
            return CurrentSession.CreateCriteria<ParentAreaGroup>()
                .Add(Restrictions.Eq("ProfileId", profileId))
                .List<ParentAreaGroup>();
        }

        public IList<ParentAreaGroup> GetParentAreaGroupsForChildAreaType(int childAreaTypeId)
        {
            return CurrentSession.CreateCriteria<ParentAreaGroup>()
                .Add(Restrictions.Eq("ChildAreaTypeId", childAreaTypeId))
                .List<ParentAreaGroup>();
        }

        public virtual IList<ParentAreaGroup> GetParentAreaGroups(int profileId, int childAreaTypeId)
        {
            return CurrentSession.CreateCriteria<ParentAreaGroup>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ProfileId", profileId))
                .Add(Restrictions.Eq("ChildAreaTypeId", childAreaTypeId))
                .List<ParentAreaGroup>();
        }

        public virtual Area GetAreaFromCode(string code)
        {
            var q = CurrentSession.CreateQuery("from Area a where a.Code = :code");
            q.SetParameter("code", code);
            var area = q.UniqueResult<Area>();
            ValidateArea(code, area);
            return area;
        }

        public virtual IList<IArea> GetAreasFromCodes(IEnumerable<string> codes)
        {
            if ((codes == null) || !codes.Any())
            {
                return new List<IArea>();
            }

            var q = CurrentSession.CreateQuery("from Area a where a.Code in (:codes)");
            q.SetParameterList("codes", codes.ToList());
            return q.List<IArea>();
        }

        public virtual IList<string> GetAreaCodesThatDoNotExist(IList<string> areaCodes)
        {
            var distinctCodes = areaCodes.Distinct().ToList();

            var splitter = new LongListSplitter<string>(distinctCodes);
            var codesThatDoNotExist = new List<string>();
            while (splitter.AnyLeft())
            {
                var codesToBeChecked = splitter.NextItems().ToList();
                var codesThatExist = GetAreaCodesThatExist(codesToBeChecked);

                // Are there any non-existant codes
                if (codesToBeChecked.Count != codesThatExist.Count)
                {
                    // All codes exist
                    codesThatDoNotExist.AddRange(
                        codesToBeChecked.Where(x => codesThatExist.Contains(x) == false));
                }
            }

            return codesThatDoNotExist;
        }

        private IList<string> GetAreaCodesThatExist(IEnumerable<string> areaCodes)
        {
            return CurrentSession.CreateCriteria<Area>()
                .Add(Restrictions.In("Code", areaCodes.ToList()))
                .SetProjection(Projections.Property("Code"))
                .List<string>();
        }

        public virtual IList<IArea> GetAreasByAreaTypeId(int areaTypeId)
        {
            return CurrentSession.CreateCriteria<IArea>()
                .SetCacheable(true)
                .Add(Restrictions.In("AreaTypeId", GetComponentAreaTypeIds(areaTypeId).ToArray()))
                .Add(Restrictions.Eq("IsCurrent", true))
                .List<IArea>();
        }

        public IList<IArea> GetAreasByAreaTypeIdAndAreaNameSearchText(int areaTypeId, string areaNameSearchText)
        {
            return CurrentSession.CreateCriteria<IArea>()
                .SetCacheable(true)
                .Add(Restrictions.In("AreaTypeId", GetComponentAreaTypeIds(areaTypeId).ToArray()))
                .Add(Restrictions.InsensitiveLike("Name", areaNameSearchText, MatchMode.Anywhere))
                .List<IArea>();
        }

        public AreaAddress GetAreaWithAddressFromCode(string code)
        {
            var q = CurrentSession.CreateQuery("from AreaAddress a where a.Code = :code");
            q.SetParameter("code", code);
            return q.UniqueResult<AreaAddress>();
        }

        /// <summary>
        ///     Gets a URL to the web site of the area, e.g. a local authority home page.
        /// </summary>
        public string GetAreaUrl(string areaCode)
        {
            var link = CurrentSession.CreateCriteria<AreaLink>()
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .UniqueResult<AreaLink>();

            return link != null
                ? link.Url
                : null;
        }

        public IList<PlaceName> GetAllPlaceNames()
        {
            return CurrentSession.CreateCriteria<PlaceName>().List<PlaceName>();
        }

        public virtual IList<PostcodeParentAreas> GetAllPostCodeParentAreasStartingWithLetter(string firstLetter)
        {
            // Need to clear the cache to prevent OutOfMemory exception
            CurrentSession.Clear();

            return CurrentSession.CreateCriteria<PostcodeParentAreas>()
                .Add(Restrictions.Like("Postcode", firstLetter + "%"))
                .List<PostcodeParentAreas>();
        }

        public IList<AreaAddress> GetAreaWithAddressFromCodes(IEnumerable<string> codes)
        {
            var q = CurrentSession.CreateQuery("from AreaAddress a where a.Code in (:codes)");
            q.SetParameterList("codes", codes.ToList());
            return q.List<AreaAddress>();
        }

        public IList<AreaAddress> GetAreaWithAddressByAreaTypeId(int areaTypeId)
        {
            var q = CurrentSession.CreateQuery("from AreaAddress a where a.AreaTypeId = :areaTypeId and a.IsCurrent = 1");
            q.SetCacheable(true);
            q.SetParameter("areaTypeId", areaTypeId);
            return q.List<AreaAddress>();
        }

        public virtual IList<IArea> GetChildAreas(string parentArea, int childAreaTypeId)
        {
            // Throws exception if SetCacheable(true)
            var q = CurrentSession.GetNamedQuery("GetChildAreas");
            q.SetParameter("parentArea", parentArea);
            q.SetParameterList("childAreaTypeIds", GetComponentAreaTypeIds(childAreaTypeId));
            return GetAreas(q).Cast<IArea>().ToList();
        }

        /// <summary>
        ///     Hash key(childAreaCode) -> value(parentArea)
        /// </summary>
        /// <param name="parentAreaTypeIds"></param>
        /// <param name="childAreaTypeId"></param>
        /// <returns></returns>
        public Dictionary<string, Area> GetParentsFromChildAreaIdAndParentAreaTypeId(int parentAreaTypeIds,
            int childAreaTypeId)
        {
            var q = CurrentSession.GetNamedQuery("GetParentsFromChildAreaIdAndParentAreaTypeId");
            q.SetParameterList("parentAreaTypeIds", GetComponentAreaTypeIds(parentAreaTypeIds));
            q.SetParameter("childAreaTypeId", childAreaTypeId);

            var results = q.List().Cast<object[]>();
            var map = GetAreaCodeToAreaMap(results);
            return map;
        }

        public Dictionary<string, Area> GetParentAreasFromChildAreaId(int parentAreaTypeId, int childAreaTypeId)
        {
            // Error thrown if SetCacheable(true);
            var q = CurrentSession.GetNamedQuery("GetAllParentsFromChildAreaId");
            q.SetParameterList("childAreaTypeIds", GetComponentAreaTypeIds(childAreaTypeId));
            q.SetParameterList("parentAreaTypeIds", GetComponentAreaTypeIds(parentAreaTypeId));

            var results = q.List().Cast<object[]>();
            var map = GetAreaCodeToAreaMap(results);
            return map;
        }

        public Dictionary<string, Area> GetParentAreasFromCategoryTypeIdAndChildAreaTypeId(int categoryTypeId, int childAreaTypeId)
        {
            var q = CurrentSession.CreateSQLQuery(@"SELECT areas.AreaCode as ParentAreaCode, areas.AreaName, areas.AreaShortName, areas.AreaTypeId, categAreas.AreaCode as ChildAreaCode
                                                 FROM L_CategorisedAreas as categAreas
                                                 INNER JOIN L_Areas as areas ON categAreas.ParentAreaTypeID = areas.AreaTypeID
                                                 WHERE categAreas.CategoryTypeID = (:categoryTypeId) AND ChildAreaTypeID = (:childAreaTypeId)");

            q.SetParameter("categoryTypeId", categoryTypeId);
            q.SetParameter("childAreaTypeId", childAreaTypeId);

            var results = q.List().Cast<object[]>();
            var map = GetAreaCodeToAreaMap(results);
            return map;
        }

        public IList<string> GetParentCodesFromChildAreaId(int childAreaTypeId)
        {
            var q = CurrentSession.CreateQuery("select distinct h.ParentAreaCode from  Area a, AreaHierarchy h" +
                                               " where a.Code = h.ChildAreaCode and a.AreaTypeId in (:childAreaTypeIds) and a.IsCurrent = 1");
            q.SetCacheable(true);
            q.SetParameterList("childAreaTypeIds", GetComponentAreaTypeIds(childAreaTypeId));
            return q.List<string>();
        }

        public IList<Area> GetParentAreas(string childAreaCode)
        {
            var q = CurrentSession.GetNamedQuery("GetParents");
            q.SetParameter("childAreaCode", childAreaCode);
            return GetAreas(q);
        }

        public virtual IList<AreaType> GetAllAreaTypes()
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("IsCurrent", true))
                .List<AreaType>();
        }

        public IList<int> GetAllAreaTypeIds()
        {
            return CurrentSession.CreateCriteria<AreaType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("IsCurrent", true))
                .SetProjection(Projections.Property("Id"))
                .List<int>();
        }

        public virtual IList<AreaType> GetSupportedAreaTypes()
        {
            return CurrentSession.QueryOver<AreaType>()
                .Where(x => x.IsSupported && x.IsCurrent)
                .List<AreaType>();
        }

        public virtual IList<AreaType> GetAreaTypes(IEnumerable<int> areaTypeIds)
        {
            var list = areaTypeIds.ToList();
            if (list.Count == 0)
            {
                return new List<AreaType>();
            }

            return CurrentSession.CreateCriteria<AreaType>()
                .SetCacheable(true)
                .Add(Restrictions.In("Id", list))
                .List<AreaType>();
        }

        public virtual AreaType GetAreaType(int areaTypeId)
        {
            var result = CurrentSession.CreateCriteria<AreaType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", areaTypeId))
                .UniqueResult<AreaType>();
            return result;
        }

        /// <summary>
        ///     Total number of areas of a given area type.
        /// </summary>
        public int GetAreaCountForAreaType(int areaTypeId)
        {
            var q =
                CurrentSession.CreateQuery(
                    "select count (a) from Area a where a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1");
            q.SetCacheable(true);
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            return Convert.ToInt32(q.UniqueResult<long>());
        }

        public int GetChildAreaCount(string parentAreaCode, int areaTypeId)
        {
            // Throws exception is SetCacheable(true)
            var q = CurrentSession.GetNamedQuery("GetChildAreaCount");
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            q.SetParameter("parentAreaCode", parentAreaCode);
            return q.UniqueResult<int>();
        }

        public IList<string> GetAllAreaCodes()
        {
            return CurrentSession.CreateCriteria<Area>()
                .SetProjection(Projections.Property("Code"))
                .List<string>();
        }

        public IList<string> GetAreaCodesForAreaType(int areaTypeId)
        {
            var q =
                CurrentSession.CreateQuery(
                    "select (a.Code) from Area a where a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1");
            q.SetCacheable(true);
            q.SetParameterList("areaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            return q.List<string>();
        }

        public IList<string> GetChildAreaCodes(string parentAreaCode, int areaTypeId)
        {
            // Error thrown if SetCacheable(true)
            var q = CurrentSession.GetNamedQuery("GetChildAreaCodes");
            q.SetParameterList("childAreaTypeIds", GetComponentAreaTypeIds(areaTypeId));
            q.SetParameter("parentAreaCode", parentAreaCode);
            return q.List<string>();
        }

        public IList<CategorisedArea> GetCategorisedAreasForAllCategories(int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<CategorisedArea>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("ParentAreaTypeId", parentAreaTypeId))
                .Add(Restrictions.Eq("ChildAreaTypeId", childAreaTypeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .List<CategorisedArea>();
            return result;
        }

        public IList<CategorisedArea> GetCategorisedAreasForOneCategory(int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId, int categoryId)
        {
            //Join to the Areas table to ensure just IsCurrent areas are selected
            var q =
                CurrentSession.CreateQuery(
                    "select ca from CategorisedArea ca, Area a where ca.AreaCode = a.Code and a.IsCurrent = 1 and ca.ParentAreaTypeId = :parentareatypeid and ca.ChildAreaTypeId = :childareatypeid" +
                    " and ca.CategoryTypeId = :categorytypeid and ca.CategoryId = :categoryid");
            q.SetParameter("parentareatypeid", parentAreaTypeId);
            q.SetParameter("childareatypeid", childAreaTypeId);
            q.SetParameter("categorytypeid", categoryTypeId);
            q.SetParameter("categoryid", categoryId);

            return q.List<CategorisedArea>();
        }

        public CategorisedArea GetCategorisedArea(string areaCode, int parentAreaTypeId, int childAreaTypeId,
            int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<CategorisedArea>()
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .Add(Restrictions.Eq("ParentAreaTypeId", parentAreaTypeId))
                .Add(Restrictions.Eq("ChildAreaTypeId", childAreaTypeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .UniqueResult<CategorisedArea>();
            return result;
        }

        public Category GetCategory(int categoryTypeId, int categoryId)
        {
            var result = CurrentSession.CreateCriteria<Category>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .Add(Restrictions.Eq("Id", categoryId))
                .UniqueResult<Category>();
            return result;
        }

        public IList<Category> GetCategories(int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<Category>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .AddOrder(Order.Asc("Id"))
                .List<Category>();
            return result;
        }

        public CategoryType GetCategoryType(int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<CategoryType>()
                .SetCacheable(true)
                .Add(Restrictions.Eq("Id", categoryTypeId))
                .UniqueResult<CategoryType>();
            return result;
        }

        public IList<CategoryType> GetCategoryTypes(IList<int> categoryTypeIds)
        {
            return CurrentSession.CreateCriteria<CategoryType>()
                .SetCacheable(true)
                .Add(Restrictions.In("Id", categoryTypeIds.ToList()))
                .List<CategoryType>();
        }

        public IList<AreaCodeNeighbourMapping> GetNearestNeighbours(string areaCode, int neighbourTypeId)
        {
            return CurrentSession.CreateCriteria<AreaCodeNeighbourMapping>()
                .Add(Restrictions.Conjunction()
                    .Add(Restrictions.Eq("AreaCode", areaCode))
                    .Add(Restrictions.Eq("NeighbourTypeId", neighbourTypeId)))
                .AddOrder(Order.Asc("Position"))
                .List<AreaCodeNeighbourMapping>();
        }

        public IList<CategoryType> GetAllCategoryTypes()
        {
            return CurrentSession.CreateCriteria<CategoryType>()
                .SetCacheable(true)
                .List<CategoryType>();
        }

        public IList<NearByAreas> GetNearbyAreas(string easting, string northing, int areaTypeId)
        {
            return CurrentSession.GetNamedQuery("GetNearbyGpPractice_SP")
                .SetString("easting", easting)
                .SetString("northing", northing)
                .SetInt32("areatypeid", areaTypeId)
                .SetResultTransformer(Transformers.AliasToBean<NearByAreas>())
                .List<NearByAreas>();
        }

        /// <summary>
        ///     Total number of areas of a given area type.
        /// </summary>
        public int GetChildAreaCount(CategoryArea categoryArea, int childAreaTypeId)
        {
            var q = CurrentSession.CreateQuery("select count (a) from CategorisedArea a " +
                                               "where a.CategoryTypeId = :categoryTypeId and a.CategoryId = :categoryId and a.ChildAreaTypeId = :childAreaTypeId and a.ParentAreaTypeId = : parentAreaTypeId");
            q.SetParameter("categoryTypeId", categoryArea.CategoryTypeId);
            q.SetParameter("categoryId", categoryArea.CategoryId);
            q.SetParameter("childAreaTypeId", childAreaTypeId);
            q.SetParameter("parentAreaTypeId", categoryArea.ParentAreaTypeId);
            return Convert.ToInt32(q.UniqueResult<long>());
        }

        private static void ValidateArea(string code, Area area)
        {
            if (area == null)
            {
                throw new FingertipsException("Area cannot be found: '" + code + "'");
            }
        }

        private IList<int> GetComponentAreaTypeIds(int areaTypeId)
        {
            return new AreaTypeIdSplitter(new AreaTypeComponentRepository())
                .GetComponentAreaTypeIds(areaTypeId);
        }

        private static IList<Area> GetAreas(IQuery q)
        {
            var list = q.List();
            return (from object[] row in list select GetAreaFromRow(row)).ToList();
        }

        private static Area GetAreaFromRow(object[] row)
        {
            return new Area
            {
                Code = (string)row[0],
                Name = (string)row[1],
                ShortName = (string)row[2],
                AreaTypeId = Convert.ToInt32(row[3])
            };
        }

        private static Dictionary<string, Area> GetAreaCodeToAreaMap(IEnumerable<object[]> results)
        {
            var map = new Dictionary<string, Area>();
            foreach (var result in results)
            {
                var areaCode = (string)result[4];
                if (map.ContainsKey(areaCode) == false)
                {
                    map.Add(areaCode, GetAreaFromRow(result));
                }
            }
            return map;
        }
    }
}