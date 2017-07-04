using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FingertipsUploadService.ProfileDataTest.Respositories
{
    [TestClass]
    public class WhenUsingDisclosureControl
    {
        [TestMethod]
        public void GetDisclosureControlTest()
        {
            var repository = new DisclosureControlRepository();
            var id = DisclosureControlIds.NoCheckRequired;
            var disclosureControl = repository.GetDisclosureControlById(id);
            Assert.AreEqual(id, disclosureControl.Id);
        }
    }
}
