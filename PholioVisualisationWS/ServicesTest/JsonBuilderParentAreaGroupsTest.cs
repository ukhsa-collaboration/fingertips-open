using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.PholioObjects;
using PholioVisualisation.RequestParameters;
using PholioVisualisation.Services;

namespace PholioVisualisation.ServicesTest
{
    [TestClass]
    public class JsonBuilderParentAreaGroupsTest
    {
        [TestMethod]
        public void When_Specific_Profile_Then_Only_Area_Types_Used_As_Child_Area_Types_Are_Included()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ProfileId, ProfileIds.Phof.ToString());
            var parameters = new ParentAreaGroupsParameters(nameValues);
            var areaTypes =  new JsonBuilderParentAreaGroups(parameters).GetChildAreaTypeIdToParentAreaTypes();

            Assert.IsTrue(areaTypes.Select(x => x.Id).ToList().Contains(AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019));
            Assert.IsFalse(areaTypes.Select(x => x.Id).ToList().Contains(AreaTypeIds.CcgsPostApr2019));
        }

        [TestMethod]
        public void When_Profile_Not_Defined_Then_All_Area_Types_Included()
        {
            NameValueCollection nameValues = new NameValueCollection();
            nameValues.Add(ParameterNames.ProfileId, ProfileIds.Undefined.ToString());
            var parameters = new ParentAreaGroupsParameters(nameValues);
            var areaTypes = new JsonBuilderParentAreaGroups(parameters).GetChildAreaTypeIdToParentAreaTypes();

            Assert.IsTrue(areaTypes.Select(x => x.Id).ToList().Contains(AreaTypeIds.CountyAndUnitaryAuthorityPreApr2019));
            Assert.IsTrue(areaTypes.Select(x => x.Id).ToList().Contains(AreaTypeIds.CcgsPostApr2019));
        }
    }
}
