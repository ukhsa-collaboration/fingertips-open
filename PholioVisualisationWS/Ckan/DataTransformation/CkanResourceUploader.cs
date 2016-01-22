using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ckan.Client;
using Ckan.Exceptions;
using Ckan.Model;

namespace Ckan.DataTransformation
{
    public interface ICkanResourceUploader
    {
        ICkanApi CkanApi { get; set; }

        /// <summary>
        /// Uploads resources to CKAN and associates them with a package.
        /// </summary>
        /// <param name="packageId">The ID of the package that owns the resources</param>
        /// <param name="resources">The resources to upload</param>
        /// <returns>The uploaded resources returned from CKAN</returns>
        IList<CkanResource> AddResourcesToPackage(string packageId,
            params CkanResource[] resources);
    }

    public class CkanResourceUploader : ICkanResourceUploader
    {
        public ICkanApi CkanApi { get; set; }
        public bool WaitIfResourceUploadFails = true;

        public IList<CkanResource> AddResourcesToPackage(string packageId, 
            params CkanResource[] resources)
        {
            IList<CkanResource> uploadedResources = new List<CkanResource>();

            int attemptCount = 0;
            while (true)
            {
                attemptCount++;
                if (attemptCount > 10)
                {
                    throw new CkanApiException("Repeated exceptions, reached retry limit");
                }

                foreach (var ckanResource in resources)
                {
                    var savedResource = UploadResource(ckanResource);
                    if (savedResource != null)
                    {
                        // Upload success
                        uploadedResources.Add(savedResource);
                    }
                    else
                    {
                        // Upload failed
                        break;
                    }
                }

                if (uploadedResources.Count != resources.Length)
                {
                    // Failed to load at least one resource
                    Console.WriteLine("#UPLOAD FAILED: will retry uploading resources for " +
                        packageId);
                    uploadedResources.Clear();
                    RemoveAllResourcesFromPackage(packageId);
                }
                else
                {
                    // All resources have been uploaded
                    break;
                }
            }

            return uploadedResources;
        }

        private void RemoveAllResourcesFromPackage(string packageId)
        {
            var package = CkanApi.GetPackage(packageId);
            package.Resources.Clear();
            CkanApi.UpdatePackage(package);
        }

        private CkanResource UploadResource(CkanResource ckanResource)
        {
            try
            {
                return CkanApi.CreateResource(ckanResource);
            }
            catch (Exception ex)
            {
                if (ex is CkanTimeoutException || ex is CkanInternalServerException)
                {
                    if (WaitIfResourceUploadFails)
                    {
                        // Wait 10s before continuing
                        Thread.Sleep(1000*10);
                    }
                }
                else
                {
                    throw;
                }
            }

            return null;
        }
    }
}
