using System.Collections.Generic;
using Ckan.Model;

namespace Ckan.Client
{
    public interface ICkanApi
    {
        List<string> GetGroupIds();
        CkanGroup CreateGroup(CkanGroup ckanGroup);

        /// <summary>
        /// Gets a CKAN group or null if it is not found.
        /// </summary>
        CkanGroup GetGroup(string id);
        
        CkanGroup UpdateGroup(CkanGroup ckanGroup);

        CkanPackage CreatePackage(CkanPackage package);

        /// <summary>
        /// Gets a CKAN package or null if it is not found.
        /// </summary>
        CkanPackage GetPackage(string id);

        CkanPackage UpdatePackage(CkanPackage ckanPackage);

        CkanResource CreateResource(CkanResource resource);

        CkanOrganisation GetOrganization(string id);
    }
}