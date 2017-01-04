using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.ServiceActions;
using PholioVisualisation.Services;
using ServicesWeb.Common;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class AreasController : BaseController
    {

        /// <summary>
        /// Get the categories of an area
        /// </summary>
        /// <remarks>
        /// Returns a dictionary of area code to category ID
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="category_type_id">Category type ID</param>
        [HttpGet]
        [Route("area_categories")]
        public Dictionary<string, int> GetAreaCategories(int profile_id, int child_area_type_id, int category_type_id)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.AreaTypeId, child_area_type_id.ToString());
                nameValues.Add(ParameterNames.CategoryTypeId, category_type_id.ToString());

                var parameters = new AreaCategoriesParameters(nameValues);
                return new JsonBuilderAreaCategories(parameters).GetAreaCodeToCategoryIdMap();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get a list of area types
        /// </summary>
        /// <remarks>
        /// If the profile_ids parameter is omitted then all defined area types will be returned.
        /// </remarks>
        /// <param name="profile_ids">List of profile IDs</param>
        [HttpGet]
        [Route("area_types")]
        public IList<AreaType> GetAreaTypes(string profile_ids = null)
        {
            try
            {
                return new AreaTypesAction().GetResponse(
                    new IntListStringParser(profile_ids).IntList);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the area types in a profile along with the available parent area types
        /// </summary>
        /// <remarks>
        /// Returns a list of area types each with a list of available parent area types
        /// </remarks>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="template_profile_id">ID of the profile to use as template if accessing search results</param>
        [HttpGet]
        [Route("area_types/parent_area_types")]
        public IList<IAreaType> GetAreaTypesWithParentAreaTypes(int profile_id = ProfileIds.Undefined, int? template_profile_id = null)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());

                if (template_profile_id.HasValue)
                {
                    nameValues.Add(ParameterNames.TemplateProfileId, template_profile_id.ToString());
                }

                var parameters = new ParentAreaGroupsParameters(nameValues);
                return new JsonBuilderParentAreaGroups(parameters).GetChildAreaTypeIdToParentAreaTypes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the address with latitude and longitude of a point area
        /// </summary>
        /// <param name="area_code">Area code</param>
        [HttpGet]
        [Route("area_address")]
        public AreaAddress GetAreaAddress(string area_code)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.AreaCode, area_code);

                var parameters = new AreaAddressParameters(nameValues);
                return new JsonBuilderAreaAddress(parameters).GetAreaAddress();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Gets a list of areas and their addresses within a parent area
        /// </summary>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("area_addresses/by_parent_area_code")]
        public IList<AreaAddress> GetChildAreasWithAddresses(string parent_area_code, int area_type_id)
        {
            try
            {
                return new ChildAreasWithAddressesBuilder().Build(parent_area_code, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of areas of a specific area type
        /// </summary>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="template_profile_id">Template profile ID (optional)</param>
        /// <param name="retrieve_ignored_areas">Whether to retrieve areas that are ignored for the profile [yes or no accepted] (optional)</param>
        [HttpGet]
        [Route("areas/by_area_type")]
        public IList<IArea> GetAreasOfAreaType(int area_type_id, int profile_id = ProfileIds.Undefined,
            int template_profile_id = ProfileIds.Undefined, string retrieve_ignored_areas = null)
        {
            try
            {
                // Get all areas of an area type
                var retrieveIgnoredAreas = ServiceHelper.ParseYesOrNo(retrieve_ignored_areas, false);

                return new AreasOfAreaTypeAction().GetResponse(area_type_id, profile_id,
                    template_profile_id, retrieveIgnoredAreas);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of areas that belong to a parent area
        /// </summary>
        /// <param name="area_type_id">Area type ID</param>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="profile_id">Profile ID (optional)</param>
        [HttpGet]
        [Route("areas/by_parent_area_code")]
        public IList<IArea> GetAreasOfAreaType(int area_type_id, string parent_area_code, int profile_id = ProfileIds.Undefined)
        {
            try
            {
                return GetChildAreas(area_type_id, parent_area_code, profile_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        ///     Get a list of areas from their area codes
        /// </summary>
        /// <param name="area_codes">List of area codes</param>
        [HttpGet]
        [Route("areas/by_area_code")]
        public IList<IArea> GetAreasOfAreaType(string area_codes)
        {
            try
            {
                var codes = new StringListParser(area_codes).StringList;
                var areaListBuilder = new AreaListProvider(ReaderFactory.GetAreasReader());
                areaListBuilder.CreateAreaListFromAreaCodes(codes);
                return areaListBuilder.Areas;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the parent areas of an area
        /// </summary>
        /// <remarks>
        /// Returns a hierarchy of parent areas
        /// </remarks>
        /// <param name="child_area_code">Area code</param>
        /// <param name="parent_area_type_ids">Comma separated list of area type IDs</param>
        [HttpGet]
        [Route("area/parent_areas")]
        public JsonBuilderParentAreas.AreaToParentsMap GetParentAreas(string child_area_code, string parent_area_type_ids)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.AreaCode, child_area_code);
                nameValues.Add(ParameterNames.AreaTypeId, parent_area_type_ids);

                var parameters = new ParentAreasParameters(nameValues);
                return new JsonBuilderParentAreas(parameters).GetChildAreaToParentsMap();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a dictionary of parent area code to a list of child area codes
        /// </summary>
        /// <param name="profile_id">Profile ID</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        /// <param name="nearest_neighbour_code">Nearest neighbour code (optional)</param>
        /// <param name="retrieve_ignored_areas">Whether to retrieve areas that are ignored for the profile [yes or no accepted] (optional)</param>
        [HttpGet]
        [Route("parent_to_child_areas")]
        public Dictionary<string, IEnumerable<string>> GetParentToChildAreaMapping(int profile_id,
            int child_area_type_id, int parent_area_type_id, string nearest_neighbour_code = null,
            string retrieve_ignored_areas = null)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.AreaTypeId, child_area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaTypeId, parent_area_type_id.ToString());
                nameValues.Add(ParameterNames.ProfileId, profile_id.ToString());
                nameValues.Add(ParameterNames.NearestNeighbourCode, nearest_neighbour_code ?? string.Empty);
                nameValues.Add(ParameterNames.RetrieveIgnoredAreas, retrieve_ignored_areas ?? string.Empty);

                var parameters = new AreaMappingParameters(nameValues);
                return new JsonBuilderAreaMapping(parameters).GetParentAreaToChildAreaDictionary();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }


        /// <summary>
        /// Gets a list of areas within 5 miles radius of a point
        /// </summary>
        /// <remarks>
        /// Only will work for point areas (e.g. GP practices) not polygon areas (e.g. CCGs)
        /// </remarks>
        /// <param name="easting">Easting</param>
        /// <param name="northing">Northing</param>
        /// <param name="area_type_id">Area type ID</param>
        [HttpGet]
        [Route("area_search_by_proximity")]
        public IList<NearByAreas> GetNearbyAreas(string easting, string northing, int area_type_id)
        {
            try
            {
                return new NearbyAreasBuilder().Build(easting, northing, area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a list of places or postcodes that match specified text
        /// </summary>
        /// <param name="search_text">Text to search for</param>
        /// <param name="polygon_area_type_id">Area type ID of the areas containing the places searched for</param>
        /// <param name="parent_areas_to_include_in_results">Area type IDs of the parent types that should be included in the search results</param>
        /// <param name="include_coordinates">Whether or not to include eastings and northings</param>
        [HttpGet]
        [Route("area_search_by_text")]
        public List<GeographicalSearchResult> GetAreaSearchResults(string search_text, int polygon_area_type_id,
            string parent_areas_to_include_in_results = "", bool include_coordinates = true)
        {
            try
            {
                NameValueCollection nameValues = new NameValueCollection();
                nameValues.Add(ParameterNames.Text, search_text);
                nameValues.Add(ParameterNames.PolygonAreaTypeId, polygon_area_type_id.ToString());
                nameValues.Add(ParameterNames.ParentAreaTypesToIncludeInResults, parent_areas_to_include_in_results);
                nameValues.Add(ParameterNames.AreEastingAndNorthingRequired, include_coordinates.ToString());

                var parameters = new AreaLookupParameters(nameValues);
                return new JsonpBuilderAreaLookup(parameters).GetGeographicalSearchResults();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the parent area types of all the child areas of a parent area
        /// </summary>
        /// <remarks>
        /// e.g. the Deprivation Decile of all the practices within a CCG
        /// </remarks>
        /// <param name="parent_area_code">Parent area code</param>
        /// <param name="child_area_type_id">Child area type ID</param>
        /// <param name="parent_area_type_id">Parent area type ID</param>
        [HttpGet]
        [Route("parent_area_of_specific_type_for_child_areas")]
        public Dictionary<string, IArea> GetParentAreasOfChildAreas(string parent_area_code,
            int child_area_type_id, int parent_area_type_id)
        {
            try
            {
                return new ParentAreasOfChildAreasBuilder().GetAreaMapping(parent_area_code,
                    child_area_type_id, parent_area_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        public IList<IArea> GetChildAreas(int area_type_id, string parent_area_code,
            int profile_id = ProfileIds.Undefined)
        {
            IAreasReader reader = ReaderFactory.GetAreasReader();

            // Get child areas
            IList<IArea> areas = new ChildAreaListBuilder(reader).GetChildAreas(parent_area_code, area_type_id);

            // Remove areas that should be ignored for the profile
            areas = IgnoredAreasFilterFactory.New(profile_id)
                .RemoveAreasIgnoredEverywhere(areas)
                .ToList();

            return areas;
        }
    }
}