using Ckan.Model;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using System.Collections.Generic;

namespace Ckan.DataTransformation
{
    /// <summary>
    /// Set the properties of the package/dataset, e.g. Title, Author, etc
    /// </summary>
    public interface ICkanPackagePropertySetter
    {
        void SetProperties(CkanPackage unsavedPackage, CkanGroup ckanGroup, ProfileParameters parameters,
            IndicatorMetadata indicatorMetadata, TimeRange timeRange);
    }

    public class CkanPackagePropertySetter : ICkanPackagePropertySetter
    {
        public void SetProperties(CkanPackage unsavedPackage, CkanGroup ckanGroup, ProfileParameters parameters,
            IndicatorMetadata indicatorMetadata, TimeRange timeRange)
        {
            SetMetadataProperties(unsavedPackage, indicatorMetadata);
            SetPackageProperties(unsavedPackage, ckanGroup, parameters);
            SetDateProperties(unsavedPackage, timeRange);
        }

        private static void SetPackageProperties(CkanPackage package, 
            CkanGroup group, ProfileParameters parameters)
        {
            string homePage = parameters.ProfileUrl;
            package.Source = homePage;
            package.Homepage = homePage;
            package.LicenseTitle = parameters.LicenceTitle;
            string emailAddress = AuthorDetails.GetEmailAddress(parameters.ProfileId);
            package.Author = emailAddress;
            package.AuthorEmail = emailAddress;
            package.Maintainer = emailAddress;
            package.MaintainerEmail = emailAddress;
            package.OwnerOrganization = parameters.OrganisationId;
            package.Resources = new List<CkanResource>();
            package.Groups = new List<CkanGroup> { @group.GetMinimalGroupForSendingToCkan() };
        }

        private static void SetMetadataProperties(CkanPackage package, IndicatorMetadata indicatorMetadata)
        {
            var htmlCleaner = new HtmlCleaner();
            IDictionary<string, string> descriptiveMetadata = indicatorMetadata.Descriptive;
            package.Title = descriptiveMetadata[IndicatorMetadataTextColumnNames.Name];
            package.Notes = htmlCleaner.RemoveHtml(descriptiveMetadata[IndicatorMetadataTextColumnNames.Definition]);
            package.Origin = htmlCleaner.RemoveHtml(descriptiveMetadata[IndicatorMetadataTextColumnNames.Source]);
        }

        private static void SetDateProperties(CkanPackage package, TimeRange timeRange)
        {
            package.CoverageStartDate = timeRange.FirstTimePeriod.Year + "-01-01";

            TimePeriod timePeriod = timeRange.LastTimePeriod;
            package.CoverageEndDate = (timePeriod.Year + timePeriod.YearRange - 1) + "-12-31";
            package.Frequency = new List<string> { new CkanFrequency(timePeriod).Frequency };
        }
    }
}