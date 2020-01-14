using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Export.FileBuilder.Dtos;
using PholioVisualisation.Export.FileBuilder.SupportModels;
using PholioVisualisation.ServicesWebTest.Helpers;

namespace PholioVisualisation.ExportTest.FileBuilder.Dtos
{
    [TestClass]
    public class InequalitySearchDtoTest
    {
        private const int KeyTest = 1;
        private const int CategoryTypeId = 1;
        private const int CategoryId = 2;
        private const int SexId = 3;
        private const int AgeId = 4;
        private InequalitySearch _inequalitySearchExpected;
        private Dictionary<int, IList<InequalitySearch>> _dicInequalitiesSearchExpected;

        [TestInitialize]
        public void StartUp()
        {
            _inequalitySearchExpected = new InequalitySearch
            {
                CategoryInequalitySearch = new CategoryInequalitySearch(CategoryTypeId, CategoryId),
                SexAgeInequalitySearch = new SexAgeInequalitySearch(SexId, AgeId)
            };

            _dicInequalitiesSearchExpected = new Dictionary<int, IList<InequalitySearch>>
            {
                { KeyTest, new List<InequalitySearch> { _inequalitySearchExpected } }
            };
        }

        [TestMethod]
        public void ShouldMapToInequalitySearch()
        {
            var inequalitySearchDto = new InequalitySearchDto
            {
                AgeId = AgeId,
                CategoryTypeId = CategoryTypeId,
                CategoryId = CategoryId,
                SexId = SexId
            };

            var inequalityMappingResult = inequalitySearchDto.MapToInequalitySearch();

            Assert.IsNotNull(inequalityMappingResult);
            AssertHelper.IsType(_inequalitySearchExpected.GetType(), inequalityMappingResult);

            Assert.AreEqual(_inequalitySearchExpected.CategoryInequalitySearch.CategoryTypeId, inequalityMappingResult.CategoryInequalitySearch.CategoryTypeId);
            Assert.AreEqual(_inequalitySearchExpected.CategoryInequalitySearch.CategoryId, inequalityMappingResult.CategoryInequalitySearch.CategoryId);
            Assert.AreEqual(_inequalitySearchExpected.SexAgeInequalitySearch.SexId, inequalityMappingResult.SexAgeInequalitySearch.SexId);
            Assert.AreEqual(_inequalitySearchExpected.SexAgeInequalitySearch.AgeId, inequalityMappingResult.SexAgeInequalitySearch.AgeId);
        }

        [TestMethod]
        public void ShouldMapDicInequalitiesSearchDtoToDicInequalitiesSearch()
        {
            var inequalitySearchDto = new InequalitySearchDto
            {
                AgeId = AgeId,
                CategoryTypeId = CategoryTypeId,
                CategoryId = CategoryId,
                SexId = SexId
            };
            var dicInequalitiesSearchDto = new Dictionary<int, IList<InequalitySearchDto>>
            {
                { KeyTest, new List<InequalitySearchDto> { inequalitySearchDto } }
            };

            var dicInequalitiesSearchResult = InequalitySearchDto.MapDicInequalitiesSearchDtoToDicInequalitiesSearch(dicInequalitiesSearchDto);

            Assert.IsNotNull(dicInequalitiesSearchResult);
            AssertHelper.IsType(_dicInequalitiesSearchExpected.GetType(), dicInequalitiesSearchResult);

            Assert.AreEqual(_dicInequalitiesSearchExpected.Keys.FirstOrDefault(), _dicInequalitiesSearchExpected.Keys.FirstOrDefault());
            Assert.AreEqual(_dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].CategoryInequalitySearch.CategoryTypeId,
                _dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].CategoryInequalitySearch.CategoryTypeId);
            Assert.AreEqual(_dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].CategoryInequalitySearch.CategoryId,
                _dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].CategoryInequalitySearch.CategoryId);
            Assert.AreEqual(_dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].SexAgeInequalitySearch.SexId, 
                _dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].SexAgeInequalitySearch.SexId);
            Assert.AreEqual(_dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].SexAgeInequalitySearch.AgeId, 
                _dicInequalitiesSearchExpected.Values.FirstOrDefault()[0].SexAgeInequalitySearch.AgeId);
        }
    }
}
