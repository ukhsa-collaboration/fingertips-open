using System;
using Ckan.Client;
using Ckan.DataTransformation;
using Ckan.Exceptions;
using Ckan.Model;

namespace Ckan.Repositories
{
    public interface ICkanPackageRepository
    {
        CkanPackage CreateOrUpdate(CkanPackage ckanPackage);
        CkanPackage RetrieveOrGetNew(IPackageIdProvider idProvider);
    }

    public class CkanPackageRepository : ICkanPackageRepository
    {
        private ICkanApi ckanApi;

        public CkanPackageRepository(ICkanApi ckanApi)
        {
            this.ckanApi = ckanApi;
        }

        public CkanPackage CreateOrUpdate(CkanPackage ckanPackage)
        {
            return ckanPackage.IsInstanceFromRepository
                ? ckanApi.UpdatePackage(ckanPackage)
                : ckanApi.CreatePackage(ckanPackage);
        }

        public CkanPackage RetrieveOrGetNew(IPackageIdProvider idProvider)
        {
            string packageId = idProvider.NextID;

            // Get package
            CkanPackage package;
            while (true)
            {
                try
                {
                    package = ckanApi.GetPackage(packageId);
                    break;
                }
                catch (CkanNotAuthorizedException)
                {
                    Console.WriteLine("#CANNOT ACCESS PACKAGE: will look for package with next ID");
                    packageId = idProvider.NextID;
                }
            }

            return package ?? new CkanPackage { Name = packageId };
        }
    }
}