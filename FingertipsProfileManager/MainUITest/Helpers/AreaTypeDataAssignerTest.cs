using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Fpm.MainUI.Helpers;
using Fpm.ProfileData.Entities.LookUps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fpm.MainUITest.Helpers
{
    [TestClass]
    public class AreaTypeDataAssignerTest
    {
        [TestMethod]
        public void WhenNullAreaTypes()
        {
            AreaTypeSelectListBuilder model = GetBuilder(null, AreaTypeSelectListBuilder.UndefinedAreaTypeId);
            AssertAreaTypesAreUndefined(model);
        }

        [TestMethod]
        public void WhenNoAreaTypes()
        {
            AreaTypeSelectListBuilder model = GetBuilder(new List<AreaType>(), AreaTypeSelectListBuilder.UndefinedAreaTypeId);
            AssertAreaTypesAreUndefined(model);
        }

        [TestMethod]
        public void WhenAreaTypeNoLongerExistsThenNoAreaTypeIsSelected()
        {
            AreaTypeSelectListBuilder builder = GetBuilder(new List<AreaType>(), 4);
            AssertAreaTypesAreUndefined(builder);
        }

        [TestMethod]
        public void WhenAreaTypeIdExistsItIsAssignedCorrectly()
        {
            int selectedAreaTypeId = 2;

            AreaTypeSelectListBuilder builder = GetBuilder(new List<AreaType>
            {
                new AreaType {Id = 1},
                new AreaType {Id = selectedAreaTypeId},
                new AreaType {Id = 3},
            }, selectedAreaTypeId);

            Assert.AreEqual(builder.SelectedAreaTypeId, selectedAreaTypeId);

            List<SelectListItem> areaTypeListItems = builder.SelectListItems.ToList();
            Assert.IsFalse(areaTypeListItems[0].Selected);
            Assert.IsTrue(areaTypeListItems[1].Selected);
            Assert.IsFalse(areaTypeListItems[2].Selected);
        }

        private static AreaTypeSelectListBuilder GetBuilder(IList<AreaType> areaTypes, int selectedAreaTypeId)
        {
            return new AreaTypeSelectListBuilder(areaTypes, selectedAreaTypeId);
        }

        private static void AssertAreaTypesAreUndefined(AreaTypeSelectListBuilder builder)
        {
            Assert.AreEqual(builder.SelectedAreaTypeId, AreaTypeSelectListBuilder.UndefinedAreaTypeId);
            Assert.IsNull(builder.SelectListItems);
        }
    }
}