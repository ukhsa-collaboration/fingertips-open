namespace Ckan.DataTransformation
{
    /// <summary>
    /// Provides IDs for CKAN packages
    /// </summary>
    public interface IPackageIdProvider
    {
        string NextID { get; }
    }

    public class PackageIdProvider : IPackageIdProvider
    {
        private readonly int indicatorId;
        private int idsProvidedCount = 1;

        public PackageIdProvider(int indicatorId)
        {
            this.indicatorId = indicatorId;
        }

        public string NextID
        {
            get {

                string version = idsProvidedCount > 1 ?
                    "-v" + idsProvidedCount :
                    string.Empty;
                idsProvidedCount++;

                return string.Format("phe-indicator-{0}{1}", 
                    indicatorId, version); 
            }
        } 
    }
}