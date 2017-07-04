using Microsoft.VisualStudio.TestTools.UnitTesting;
using PholioVisualisation.Formatting;

namespace PholioVisualisation.ExportTest
{
    [TestClass]
    public class HtmlCleanerTest
    {
        [TestMethod]
        public void TestRemoveHtml()
        {
            var htmlCleaner = new HtmlCleaner();
            Assert.AreEqual(1, htmlCleaner.RemoveHtml("<p>a</p>").Length);
        }

        [TestMethod]
        public void TestEncoding()
        {
            var htmlCleaner = new HtmlCleaner();
            Assert.AreEqual("a", htmlCleaner.RemoveHtml("<p>a&nbsp;</p>"));
            Assert.AreEqual("&", htmlCleaner.RemoveHtml("<p>&amp;</p>"));
            Assert.AreEqual("£", htmlCleaner.RemoveHtml("<p>&pound;</p>"));
        }

        [TestMethod]
        public void TestEncodingWithTextBlock()
        {
            var html =
                @"The Public Health Outcomes Framework&nbsp;Healthy lives, healthy people: Improving outcomes and supporting transparency&nbsp;sets out a vision for public health, desired outcomes and the indicators that will help us understand how well public health is being improved and protected.&nbsp;The framework concentrates on two high-level outcomes to be achieved across the public health system, and groups further indicators into four &lsquo;domains&rsquo; that cover the full spectrum of public health. The outcomes reflect a focus not only on how long people live, but on how well they live at all stages of life.&nbsp;The data published in the tool are the baselines for the Public Health Outcomes Framework, with more recent and historical trend data where these are available. The baseline period is 2010 or equivalent, unless these data are unavailable or not deemed to be of sufficient quality.";

            var noHtml = new HtmlCleaner().RemoveHtml(html);

            Assert.IsFalse(noHtml.Contains("&nbsp;"));
            Assert.IsFalse(noHtml.Contains("&rsquo;"));
            Assert.IsFalse(noHtml.Contains("&lsquo;"));
        }

        [TestMethod]
        public void TestRemoveVoidHtmlElements()
        {
            var htmlCleaner = new HtmlCleaner();
            Assert.AreEqual(1, htmlCleaner.RemoveHtml("<img />a").Length);
        }

        [TestMethod]
        public void TestLessThansAreIgnored()
        {
            var html =
                @"<p>2005-2006: Small numbers (&lt;5) are suppressed but have been used throughout all calculations. The counts and denominators for the City of London and Isles of Scilly have been combined with Hackney and Cornwall respectively.</p>";
            var htmlCleaned = new HtmlCleaner().RemoveHtml(html);
            Assert.IsTrue(htmlCleaned.Contains("Cornwall"));
        }

        [TestMethod]
        public void TestNonBreakingSpacesRemoved()
        {
            var html = @"a.&nbsp;b";
            var htmlCleaned = new HtmlCleaner().RemoveHtml(html);
            Assert.AreEqual("a.b", htmlCleaned);
        }

        [TestMethod]
        public void TestTransformLinks()
        {
            var htmlCleaner = new HtmlCleaner();

            const string expected = "url";

            // Single quotes
            Assert.AreEqual(expected, htmlCleaner.TransformLinks("<a href='url'>text</a>"));

            // Double quotes
            Assert.AreEqual(expected, htmlCleaner.TransformLinks("<a href=\"url\">text</a>"));

            // Uppercase
            Assert.AreEqual(expected, htmlCleaner.TransformLinks("<A href='url'>text</A>"));

            // 2 URls
            Assert.AreEqual("url  url2", htmlCleaner.TransformLinks("<a href=\"url\">b</a><a href=\"url2\">c</a>"));

            // URLs separated from adjacent text
            Assert.AreEqual("ww url  url2 ww", htmlCleaner.TransformLinks("ww<a href=\"url\">b</a><a href=\"url2\">c</a>ww"));
        }
    }
}
