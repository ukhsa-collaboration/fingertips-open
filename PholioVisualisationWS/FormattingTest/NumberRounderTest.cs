using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;


namespace PholioVisualisation.FormattingTest
{
    [TestClass]
    public class NumberRounderTest
    {
        [TestMethod]
        public void Nearest10Test_1()
        {
            Assert.AreEqual(120, NumberRounder.ToNearest10(124));
        }

        [TestMethod]
        public void Nearest10Test_2()
        {
            Assert.AreEqual(130, NumberRounder.ToNearest10(126));
        }

        [TestMethod]
        public void Nearest10Test_3()
        {
            Assert.AreEqual(130, NumberRounder.ToNearest10(125));
        }

        [TestMethod]
        public void Nearest100Test_1()
        {
            Assert.AreEqual(12400, NumberRounder.ToNearest100(12410));
        }

        [TestMethod]
        public void Nearest100Test_3333()
        {
            Assert.AreEqual(3300, NumberRounder.ToNearest100(3333));
        }

        [TestMethod]
        public void Nearest100Test_2()
        {
            Assert.AreEqual(1300, NumberRounder.ToNearest100(1260));
        }

        [TestMethod]
        public void Nearest100Test_3()
        {
            Assert.AreEqual(1300, NumberRounder.ToNearest100(1250));
        }

        [TestMethod]
        public void Nearest1000Test_1()
        {
            Assert.AreEqual(12000, NumberRounder.ToNearest1000(12410));
        }

        [TestMethod]
        public void Nearest1000Test_2()
        {
            Assert.AreEqual(265000, NumberRounder.ToNearest1000(265101));
        }

        [TestMethod]
        public void Nearest1000Test_3()
        {
            Assert.AreEqual(2651000, NumberRounder.ToNearest1000(2651090));
        }
    }
}
