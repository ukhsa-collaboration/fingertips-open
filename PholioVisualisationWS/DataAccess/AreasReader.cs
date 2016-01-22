
using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess
{
    public interface IAreasReader
    {
        IList<ParentAreaGroup> GetParentAreaGroups(int profileId);
        Area GetAreaFromCode(string code);
        IList<Area> GetAreasFromCodes(IEnumerable<string> codes);
        IList<Area> GetAreasByAreaTypeId(int areaTypeId);
        AreaAddress GetAreaWithAddressFromCode(string code);

        /// <summary>
        /// Gets a URL to the web site of the area, e.g. a local authority home page.
        /// </summary>
        string GetAreaUrl(string areaCode);

        IList<PlaceName> GetAllPlaceNames();
        IList<PostcodeParentAreas> GetAllPostCodeParentAreasStartingWithLetter(string firstLetter);
        IList<AreaAddress> GetAreaWithAddressFromCodes(IEnumerable<string> codes);
        IList<AreaAddress> GetAreaWithAddressByAreaTypeId(int areaTypeId);
        IList<Area> GetChildAreas(string parentArea, int childAreaTypeId);

        /// <summary>
        /// Hash key(childAreaCode) -> value(parentArea)
        /// </summary>
        /// <param name="parentAreaTypeIds"></param>
        /// <param name="childAreaTypeId"></param>
        /// <returns></returns>
        Dictionary<string, Area> GetParentsFromChildAreaIdAndParentAreaTypeId(int parentAreaTypeIds, int childAreaTypeId);

        Dictionary<string, Area> GetParentAreasFromChildAreaId(int childAreaTypeId);
        IList<string> GetParentCodesFromChildAreaId(int childAreaTypeId);
        IList<Area> GetParentAreas(string childAreaCode);
        IList<AreaType> GetAreaTypes(IEnumerable<int> areaTypeIds);
        AreaType GetAreaType(int areaTypeId);

        /// <summary>
        /// Total number of areas of a given area type.
        /// </summary>
        int GetAreaCountForAreaType(int areaTypeId);
        int GetChildAreaCount(string parentAreaCode, int areaTypeId);
        int GetChildAreaCount(CategoryArea categoryArea, int childAreaTypeId);

        IList<string> GetAreaCodesForAreaType(int areaTypeId);
        IList<string> GetProfileParentAreaCodes(int profileId, int parentAreaTypeId);
        IList<string> GetChildAreaCodes(string parentAreaCode, int areaTypeId);
        IList<CategorisedArea> GetCategorisedAreasForAllCategories(int parentAreaTypeId, int childAreaTypeId, int categoryTypeId);
        IList<CategorisedArea> GetCategorisedAreasForOneCategory(int parentAreaTypeId, int childAreaTypeId, int categoryTypeId, int categoryId);
        CategorisedArea GetCategorisedArea(string areaCode, int parentAreaTypeId, int childAreaTypeId, int categoryTypeId);
        Category GetCategory(int categoryTypeId, int categoryId);
        IList<Category> GetCategories(int categoryTypeId);
        CategoryType GetCategoryType(int categoryTypeId);
        IList<CategoryType> GetCategoryTypes(IList<int> categoryTypeIds);
        string GetNhsChoicesAreaId(string areaCode);
        IList<NearByAreas> GetNearbyAreas(string easting, string northing, int areaTypeId);
        IList<AreaCodeNeighbourMapping> GetNearestNeighbours(string areaCode);

        /// <summary>
        /// Opens a data access session
        /// </summary>
        /// <exception cref="Exception">Thrown if an error occurs while opening the session</exception>
        void OpenSession();

        /// <summary>
        /// IDisposable.Dispose implementation (closes and disposes of the encapsulated session)
        /// </summary>
        void Dispose();
    }

    public class AreasReader : BaseReader, IAreasReader
    {
        /// <summary>
        /// Parameterless constructor to allow mocking
        /// </summary>
        public AreasReader() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionFactory">The session factory</param>
        public AreasReader(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
        }

        public virtual IList<ParentAreaGroup> GetParentAreaGroups(int profileId)
        {
            return CurrentSession.CreateCriteria<ParentAreaGroup>()
            .Add(Restrictions.Eq("ProfileId", profileId))
            .List<ParentAreaGroup>();
        }

        public virtual Area GetAreaFromCode(string code)
        {
            IQuery q = CurrentSession.CreateQuery("from Area a where a.Code = :code");
            q.SetParameter("code", code);
            var area = q.UniqueResult<Area>();
            ValidateArea(code, area);
            return area;
        }

        private static void ValidateArea(string code, Area area)
        {
            if (area == null)
            {
                throw new FingertipsException("Area cannot be found: " + code);
            }
        }

        public virtual IList<Area> GetAreasFromCodes(IEnumerable<string> codes)
        {
            if ((codes == null) || !codes.Any())
            {
                return new List<Area>();
            }

            IQuery q = CurrentSession.CreateQuery("from Area a where a.Code in (:codes)");
            q.SetParameterList("codes", codes.ToList());
            return q.List<Area>();
        }

        public virtual IList<Area> GetAreasByAreaTypeId(int areaTypeId)
        {
            return CurrentSession.CreateCriteria<Area>()
                .Add(Restrictions.In("AreaTypeId", new AreaTypeIdSplitter(areaTypeId).Ids))
                .Add(Restrictions.Eq("IsCurrent", true))
                .List<Area>();
        }

        public AreaAddress GetAreaWithAddressFromCode(string code)
        {
            IQuery q = CurrentSession.CreateQuery("from AreaAddress a where a.Code = :code");
            q.SetParameter("code", code);
            return q.UniqueResult<AreaAddress>();
        }

        /// <summary>
        /// Gets a URL to the web site of the area, e.g. a local authority home page.
        /// </summary>
        public string GetAreaUrl(string areaCode)
        {
            var link = CurrentSession.CreateCriteria<AreaLink>()
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .UniqueResult<AreaLink>();

            return link != null ?
                link.Url :
                null;
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
            IQuery q = CurrentSession.CreateQuery("from AreaAddress a where a.Code in (:codes)");
            q.SetParameterList("codes", codes.ToList());
            return q.List<AreaAddress>();
        }

        public IList<AreaAddress> GetAreaWithAddressByAreaTypeId(int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("from AreaAddress a where a.AreaTypeId = :areaTypeId and a.IsCurrent = 1");
            q.SetParameter("areaTypeId", areaTypeId);
            return q.List<AreaAddress>();
        }

        public virtual IList<Area> GetChildAreas(string parentArea, int childAreaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetChildAreas");
            q.SetParameter("parentArea", parentArea);
            q.SetParameterList("childAreaTypeIds", new AreaTypeIdSplitter(childAreaTypeId).Ids);
            return GetAreas(q);
        }

        /// <summary>
        /// Hash key(childAreaCode) -> value(parentArea)
        /// </summary>
        /// <param name="parentAreaTypeIds"></param>
        /// <param name="childAreaTypeId"></param>
        /// <returns></returns>
        public Dictionary<string, Area> GetParentsFromChildAreaIdAndParentAreaTypeId(int parentAreaTypeIds, int childAreaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetParentsFromChildAreaIdAndParentAreaTypeId");
            q.SetParameterList("parentAreaTypeIds", new AreaTypeIdSplitter(parentAreaTypeIds).Ids);
            q.SetParameter("childAreaTypeId", childAreaTypeId);

            return q.List().Cast<object[]>().ToDictionary(row => (string)row[4], GetAreaFromRow);
        }

        public Dictionary<string, Area> GetParentAreasFromChildAreaId(int childAreaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetAllParentsFromChildAreaId");
            q.SetParameter("childAreaTypeId", childAreaTypeId);

            return q.List().Cast<object[]>().ToDictionary(row => (string)row[4], GetAreaFromRow);
        }

        public IList<string> GetParentCodesFromChildAreaId(int childAreaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("select distinct h.ParentAreaCode from  Area a, AreaHierarchy h" +
            " where a.Code = h.ChildAreaCode and a.AreaTypeId in (:childAreaTypeIds) and a.IsCurrent = 1");

            q.SetParameterList("childAreaTypeIds", new AreaTypeIdSplitter(childAreaTypeId).Ids);
            return q.List<string>();
        }

        public IList<Area> GetParentAreas(string childAreaCode)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetParents");
            q.SetParameter("childAreaCode", childAreaCode);
            return GetAreas(q);
        }

        public virtual IList<AreaType> GetAreaTypes(IEnumerable<int> areaTypeIds)
        {
            var list = areaTypeIds.ToList();
            if (list.Count == 0)
            {
                return new List<AreaType>();
            }

            return CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.In("Id", list))
                .List<AreaType>();
        }

        public virtual AreaType GetAreaType(int areaTypeId)
        {
            var result = CurrentSession.CreateCriteria<AreaType>()
                .Add(Restrictions.Eq("Id", areaTypeId))
                .UniqueResult<AreaType>();
            return result;
        }

        /// <summary>
        /// Total number of areas of a given area type.
        /// </summary>
        public int GetAreaCountForAreaType(int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("select count (a) from Area a where a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1");
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
            return Convert.ToInt32(q.UniqueResult<long>());
        }

        public int GetChildAreaCount(string parentAreaCode, int areaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetChildAreaCount");
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
            q.SetParameter("parentAreaCode", parentAreaCode);
            return q.UniqueResult<int>();
        }

        public IList<string> GetAreaCodesForAreaType(int areaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("select (a.Code) from Area a where a.AreaTypeId in (:areaTypeIds) and a.IsCurrent = 1");
            q.SetParameterList("areaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
            return q.List<string>();
        }

        public virtual IList<string> GetProfileParentAreaCodes(int profileId, int parentAreaTypeId)
        {
            IQuery q = CurrentSession.CreateQuery("select a from ProfileParentAreas a where a.ParentAreaTypeId = :areaTypeId and a.ProfileId = :profileId");
            q.SetParameter("areaTypeId", parentAreaTypeId);
            q.SetParameter("profileId", profileId);
            var result = q.UniqueResult<ProfileParentAreas>();
            if (result != null)
            {
                result.Init();
                return result.ParentAreaCodes;
            }
            return new List<string>();
        }

        public IList<string> GetChildAreaCodes(string parentAreaCode, int areaTypeId)
        {
            IQuery q = CurrentSession.GetNamedQuery("GetChildAreaCodes");
            q.SetParameterList("childAreaTypeIds", new AreaTypeIdSplitter(areaTypeId).Ids);
            q.SetParameter("parentAreaCode", parentAreaCode);
            return q.List<string>();
        }

        public IList<CategorisedArea> GetCategorisedAreasForAllCategories(int parentAreaTypeId, int childAreaTypeId, int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<CategorisedArea>()
                .Add(Restrictions.Eq("ParentAreaTypeId", parentAreaTypeId))
                .Add(Restrictions.Eq("ChildAreaTypeId", childAreaTypeId))
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .List<CategorisedArea>();
            return result;
        }


        public IList<CategorisedArea> GetCategorisedAreasForOneCategory(int parentAreaTypeId, int childAreaTypeId, int categoryTypeId, int categoryId)
        {

            //Join to the Areas table to ensure just IsCurrent areas are selected
            IQuery q = CurrentSession.CreateQuery("select ca from CategorisedArea ca, Area a where ca.AreaCode = a.Code and a.IsCurrent = 1 and ca.ParentAreaTypeId = :parentareatypeid and ca.ChildAreaTypeId = :childareatypeid" +
                                                  " and ca.CategoryTypeId = :categorytypeid and ca.CategoryId = :categoryid");
            q.SetParameter("parentareatypeid", parentAreaTypeId);
            q.SetParameter("childareatypeid", childAreaTypeId);
            q.SetParameter("categorytypeid", categoryTypeId);
            q.SetParameter("categoryid", categoryId);

            return q.List<CategorisedArea>();
        }

        public CategorisedArea GetCategorisedArea(string areaCode, int parentAreaTypeId, int childAreaTypeId, int categoryTypeId)
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
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .Add(Restrictions.Eq("CategoryId", categoryId))
                .UniqueResult<Category>();
            return result;
        }

        public IList<Category> GetCategories(int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<Category>()
                .Add(Restrictions.Eq("CategoryTypeId", categoryTypeId))
                .AddOrder(Order.Asc("CategoryId"))
                .List<Category>();
            return result;
        }

        public CategoryType GetCategoryType(int categoryTypeId)
        {
            var result = CurrentSession.CreateCriteria<CategoryType>()
                .Add(Restrictions.Eq("Id", categoryTypeId))
                .UniqueResult<CategoryType>();
            return result;
        }

        public IList<CategoryType> GetCategoryTypes(IList<int> categoryTypeIds)
        {
            return CurrentSession.CreateCriteria<CategoryType>()
            .Add(Restrictions.In("Id", categoryTypeIds.ToList()))
            .List<CategoryType>();
        }

        private static IList<Area> GetAreas(IQuery q)
        {
            System.Collections.IList list = q.List();
            return (from object[] row in list select GetAreaFromRow(row)).ToList();
        }

        private static Area GetAreaFromRow(object[] row)
        {
            return new Area
            {
                Code = (string)row[0],
                Name = (string)row[1],
                ShortName = (string)row[2],
                AreaTypeId = (int)row[3],
            };
        }

        public string GetNhsChoicesAreaId(string areaCode)
        {
            var mapping = CurrentSession.CreateCriteria<AreaCodeNhsMapping>()
                .Add(Restrictions.Eq("AreaCode", areaCode))
                .UniqueResult<AreaCodeNhsMapping>();

            return mapping == null ? string.Empty : mapping.NhsChoicesId;
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

        public IList<AreaCodeNeighbourMapping> GetNearestNeighbours(string areaCode)
        {
            return CurrentSession.CreateCriteria<AreaCodeNeighbourMapping>()
                                    .Add(Restrictions.Eq("AreaCode", areaCode))
                                    .AddOrder(Order.Asc("Position"))
                                    .List<AreaCodeNeighbourMapping>();
        }

        /// <summary>
        /// Total number of areas of a given area type.
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
    }
}
