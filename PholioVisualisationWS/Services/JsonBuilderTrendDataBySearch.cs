﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.DataSorting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;

namespace PholioVisualisation.Services
{
    public class JsonBuilderTrendDataBySearch 
    {
        private TrendDataBySearchParameters _parameters;

        public JsonBuilderTrendDataBySearch(TrendDataBySearchParameters parameters)
        {
            _parameters = parameters;
        }

        public IList<TrendRoot> GetTrendData()
        {
            int profileId = _parameters.ProfileId;

            var parentArea = new ParentArea(_parameters.ParentAreaCode, _parameters.AreaTypeId);
            ComparatorMap comparatorMap = new ComparatorMapBuilder(parentArea).ComparatorMap;

            // Do not repository as do not want results cached like this (need to be 
            // cached by ID and areatype, i.e. repository by roots)
            GroupData data = new GroupDataBuilderByIndicatorIds
            {
                IndicatorIds = _parameters.IndicatorIds,
                ComparatorMap = comparatorMap,
                AreaTypeId = _parameters.AreaTypeId,
                RestrictSearchProfileIds = _parameters.RestrictResultsToProfileIdList,
                ProfileId = profileId
            }.Build();

            if (data.IsDataOk)
            {
                var groupDataReader = ReaderFactory.GetGroupDataReader();
                data.GroupRoots = new GroupRootFilter(groupDataReader).RemoveRootsWithoutChildAreaData(data.GroupRoots);
            }

            bool isParentAreaCodeNearestNeighbour = Area.IsNearestNeighbour(_parameters.ParentAreaCode);

            IList<TrendRoot> trendRoots = new TrendRootBuilder().Build(data.GroupRoots, comparatorMap,
                _parameters.AreaTypeId, profileId, data.IndicatorMetadata, isParentAreaCodeNearestNeighbour);

            return trendRoots;
        }
    }
}