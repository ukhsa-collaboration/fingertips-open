﻿using System;
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
    public class JsonBuilderAreaCategories : JsonBuilderBase
    {
        private AreaCategoriesParameters parameters;

        public JsonBuilderAreaCategories(HttpContextBase context)
            : base(context)
        {
            parameters = new AreaCategoriesParameters(context.Request.Params);
            Parameters = parameters;
        }

        public override string GetJson()
        {
            Dictionary<string, int> areaCodeToCategoryIdMap = new AreaCodeToCategoryIdMapBuilder
            {
                ChildAreaTypeId = parameters.ChildAreaTypeId,
                CategoryTypeId = parameters.CategoryTypeId
            }.Build();

            /* TODO #159
             * if no categories found in database then may need to read
             *  values from PHOLIO and calculate quintiles */

            var areaCodes = ReaderFactory.GetProfileReader()
                .GetAreaCodesToIgnore(parameters.ProfileId)
                .AreaCodesIgnoredEverywhere;

            var filteredMap = areaCodeToCategoryIdMap
                .Where(x => areaCodes.Contains(x.Key) == false)
                .ToDictionary(x => x.Key, x => x.Value);

            return JsonConvert.SerializeObject(filteredMap);
        }
    }
}