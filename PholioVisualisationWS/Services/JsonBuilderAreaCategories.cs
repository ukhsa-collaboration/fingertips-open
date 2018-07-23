using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    /// <summary>
    /// Area categories are specific to each area (e.g. Deprivation decile) and do not vary by 
    /// factors such as sex, age, time period or if they do are an
    /// intrinsic property of the category.
    /// </summary>
    public class JsonBuilderAreaCategories 
    {
        private AreaCategoriesParameters _parameters;

        public JsonBuilderAreaCategories(AreaCategoriesParameters parameters)
        {
            _parameters = parameters;
        }

        public Dictionary<string, int> GetAreaCodeToCategoryIdMap()
        {
            Dictionary<string, int> areaCodeToCategoryIdMap = new AreaCodeToCategoryIdMapBuilder
            {
                ChildAreaTypeId = _parameters.ChildAreaTypeId,
                CategoryTypeId = _parameters.CategoryTypeId
            }.Build();

            /* TODO #159
             * if no categories found in database then may need to read
             *  values from PHOLIO and calculate quintiles */

            var areaCodes = ReaderFactory.GetProfileReader()
                .GetAreaCodesToIgnore(_parameters.ProfileId)
                .AreaCodesIgnoredEverywhere;

            var filteredMap = areaCodeToCategoryIdMap
                .Where(x => areaCodes.Contains(x.Key) == false)
                .ToDictionary(x => x.Key, x => x.Value);

            return filteredMap;
        }
    }
}