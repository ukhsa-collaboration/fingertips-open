using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Controllers;
using Fpm.MainUI.ViewModels;
using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainUITest.Controllers
{
    [TestClass]
    public class WhenUsingProfilesAndIndicatorsController
    {
        private CoreDataRepository _coreDataRepository;
        private LookUpsRepository _lookUpsRepository;
        private ProfileRepository _profileRepository;
        private ProfilesAndIndicatorsController _controller;

        [TestInitialize]
        public void Init()
        {
            _coreDataRepository = new CoreDataRepository();
            _lookUpsRepository = new LookUpsRepository();
            _profileRepository = new ProfileRepository();
        }

        [TestCleanup]
        public void CleanUp()
        {
            _profileRepository.Dispose();
            _coreDataRepository.Dispose();
            _lookUpsRepository.Dispose();
        }

        [TestMethod]
        public void BrowseIndicatorData_Returns_Result_With_PopulatedFilters_For_a_SelectedIndicator()
        {
            // Arrange
            _controller = new ProfilesAndIndicatorsController(_profileRepository, _lookUpsRepository, _coreDataRepository);
            
            const int indicatorId = IndicatorIds.LifeExpectancyAtBirth;
            
            // Act
            var result = (PartialViewResult)_controller.BrowseIndicatorData(indicatorId);

            // Assert
            Assert.IsTrue(result != null);

            var categoryTypes = result.ViewBag.CategoryTypeId as SelectList;
            var areaTypes = result.ViewBag.AreaTypeId as SelectList;
            var sexes = result.ViewBag.SexId as SelectList;
            var ages = result.ViewBag.AgeId as SelectList;
            var yearRanges = result.ViewBag.YearRange as SelectList;
            var years = result.ViewBag.Year as SelectList;
            var months = result.ViewBag.Month as SelectList;
            
            Assert.IsTrue(categoryTypes != null && categoryTypes.Any()
                && areaTypes != null && areaTypes.Any()
                && sexes != null && sexes.Any()
                && ages != null && ages.Any()
                && yearRanges != null && yearRanges.Any()
                && years != null && years.Any()
                && months != null && months.Any()
                );

            result = (PartialViewResult)_controller.BrowseIndicatorData(IndicatorIds.MrsaBloodstreamInfections);
            
            var quarters = result.ViewBag.Quarter as SelectList;

            Assert.IsTrue(quarters != null && quarters.Any());
        }

        [TestMethod]
        public void BrowseIndicatorData_Returns_Result_With_CoreData_For_a_SelectedIndicator()
        {
            // Arrange
            _controller = new ProfilesAndIndicatorsController(_profileRepository, _lookUpsRepository, _coreDataRepository);

            const int indicatorId = IndicatorIds.IDAOPI;

            // Act
            var result = (PartialViewResult)_controller.BrowseIndicatorData(indicatorId);

            // Assert
            Assert.IsTrue(result != null);

            var model = result.Model as BrowseDataViewModel;

            Assert.IsTrue(model != null 
                && model.IndicatorId == indicatorId 
                && model.Results.DataSet != null
                && model.Results.DataSet.Any()
                );
        }

        public void BrowseIndicatorData_Returns_Empty_Data_For_Invalid_IndicatorId()
        {
            // Arrange
            _controller = new ProfilesAndIndicatorsController(_profileRepository, _lookUpsRepository, _coreDataRepository);

            const int indicatorId = -11111999;

            // Act
            var result = _controller.BrowseIndicatorData(indicatorId) as ViewResult;

            
            // Assert
            Assert.IsTrue(result != null);

            var categoryTypes = result.ViewBag.CategoryTypeId as SelectList;

            Assert.IsTrue(categoryTypes != null && categoryTypes.Any() == false);

            var model = result.Model as BrowseDataViewModel;

            Assert.IsTrue(model != null && model.Results.DataSet.Any() == false);
        }


    }
}
