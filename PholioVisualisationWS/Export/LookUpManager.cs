﻿using PholioVisualisation.DataAccess;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.UserData;
using PholioVisualisation.UserData.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PholioVisualisation.Export
{
    /// <summary>
    /// Manages dictionaries of PHOLIO IDs to names.
    /// </summary>
    public class LookUpManager
    {
        private Dictionary<int, string> ageIdToName;
        private Dictionary<int, string> sexIdToName;
        private Dictionary<string, string> areaCodeToName;
        private Dictionary<string, string> areaCodeToTypeName;
        private Dictionary<string, int> areaCodeToTypeId;
        private Dictionary<int, string> valueNoteIdToText;
        private Dictionary<int, string> categoryTypeIdToName;
        private Dictionary<int, Dictionary<int, string>> categoryTypeIdToCategoryIdToName;

        public LookUpManager(PholioReader pholioReader, IAreasReader areasReader,
        IList<int> areaTypeIds, IList<int> categoryTypeIds, string areaListPublicId = null)
        {
            ageIdToName = pholioReader.GetAllAges().ToDictionary(x => x.Id, x => x.Name);
            sexIdToName = pholioReader.GetAllSexes().ToDictionary(x => x.Id, x => x.Name);
            InitAreaCodeToNameLookUp(areasReader, areaTypeIds);
            InitAreaListCodeToNameLookUp(areaListPublicId);

            var categoryTypes = areasReader.GetCategoryTypes(categoryTypeIds);
            categoryTypeIdToName = categoryTypes.ToDictionary(x => x.Id, x => x.Name);
            categoryTypeIdToCategoryIdToName = GetCategoryTypeIdToCategoryIdToNameLookUp(categoryTypes);

            valueNoteIdToText = pholioReader.GetAllValueNotes().ToDictionary(x => x.Id, x => x.Text);
        }

        private void InitAreaCodeToNameLookUp(IAreasReader areasReader,
            IList<int> areaTypeIds)
        {
            areaCodeToName = new Dictionary<string, string>();
            areaCodeToTypeName = new Dictionary<string, string>();
            areaCodeToTypeId = new Dictionary<string, int>();
            foreach (var areaTypeId in areaTypeIds)
            {
                var areaType = areasReader.GetAreaType(areaTypeId);
                var areas = areasReader.GetAreasByAreaTypeId(areaTypeId);
                foreach (var area in areas)
                {
                    var areaCode = area.Code;
                    if (areaCodeToName.ContainsKey(areaCode) == false)
                    {
                        areaCodeToTypeName.Add(areaCode, areaType.ShortName);
                        areaCodeToName.Add(areaCode, area.Name);
                        areaCodeToTypeId.Add(areaCode, areaTypeId);
                    }
                }
            }
        }

        private void InitAreaListCodeToNameLookUp(string AreaListPublicId)
        {
            if (AreaListPublicId != null && areaCodeToName.ContainsKey(AreaListPublicId) == false)
            {
                var areaListRepository = new AreaListRepository(new fingertips_usersEntities());
                var areaList = areaListRepository.GetAreaListByPublicId(AreaListPublicId);
                areaCodeToTypeName.Add(AreaListPublicId, "Area list");
                areaCodeToName.Add(AreaListPublicId, areaList.ListName);
            }
        }

        private static Dictionary<int, Dictionary<int, string>> GetCategoryTypeIdToCategoryIdToNameLookUp(IList<CategoryType> categoryTypes)
        {
            var categoryTypeIdToCategoryIdToName = new Dictionary<int, Dictionary<int, string>>();
            foreach (var categoryType in categoryTypes)
            {
                var categoryIdToName = categoryType.Categories.ToDictionary(x => x.Id, x => x.Name);
                categoryTypeIdToCategoryIdToName.Add(categoryType.Id, categoryIdToName);
            }
            return categoryTypeIdToCategoryIdToName;
        }

        public string GetCategoryTypeName(int categoryTypeId)
        {
            if (categoryTypeId == -1)
            {
                return "";
            }
            return categoryTypeIdToName[categoryTypeId];
        }

        public string GetCategoryName(int categoryTypeId, int categoryId)
        {
            if (categoryTypeId == -1)
            {
                return "";
            }
            return categoryTypeIdToCategoryIdToName[categoryTypeId][categoryId];
        }

        public string GetValueNoteText(int valueNoteId)
        {
            return valueNoteIdToText[valueNoteId];
        }

        public string GetAreaName(string areaCode)
        {
            if (areaCodeToName.ContainsKey(areaCode))
            {
                return areaCodeToName[areaCode];
            }

            // Try lower case code e.g. area list 
            var lcAreaCode = areaCode.ToLower();
            if (areaCodeToName.ContainsKey(lcAreaCode))
            {
                return areaCodeToName[lcAreaCode];
            }

            throw new FingertipsException(String.Format("Could not find area name for area code {0}.", areaCode));
        }

        public string GetAreaTypeName(string areaCode)
        {
            if (areaCodeToTypeName.ContainsKey(areaCode))
            {
                return areaCodeToTypeName[areaCode];
            }

            // Try lower case code e.g. area list type
            var lcAreaCode = areaCode.ToLower();
            if (areaCodeToTypeName.ContainsKey(lcAreaCode))
            {
                return areaCodeToTypeName[lcAreaCode];
            }

            throw new FingertipsException("Could not find area type name for " + areaCode);
        }

        public int GetAreaTypeId(string areaCode)
        {
            if (areaCodeToTypeId.ContainsKey(areaCode))
            {
                return areaCodeToTypeId[areaCode];
            }

            // Try lower case code e.g. area list type
            var lcAreaCode = areaCode.ToLower();
            if (areaCodeToTypeId.ContainsKey(lcAreaCode))
            {
                return areaCodeToTypeId[lcAreaCode];
            }

            throw new FingertipsException("Could not find area type id for " + areaCode);
        }

        public string GetSexName(int sexId)
        {
            return sexIdToName[sexId];
        }

        public string GetAgeName(int ageId)
        {
            return ageIdToName[ageId];
        }
    }
}
