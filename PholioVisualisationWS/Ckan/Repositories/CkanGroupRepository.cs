using System;
using System.Threading;
using Ckan.Client;
using Ckan.Exceptions;
using Ckan.Model;
using PholioVisualisation.DataAccess;
using PholioVisualisation.DataConstruction;
using PholioVisualisation.PholioObjects;

namespace Ckan.Repositories
{
    public interface ICkanGroupRepository
    {
        CkanGroup CreateOrRetrieveGroup(Profile profile);
        CkanGroup GetExistingGroup(string groupId);
        CkanGroup UpdateGroupWithProfileProperties(Profile profile, CkanGroup ckanGroup);
    }

    public class CkanGroupRepository : ICkanGroupRepository
    {
        public bool WaitIfResourceUploadFails = true;

        private ICkanApi _ckanApi;
        private IContentProvider _contentProvider;

        public CkanGroupRepository(ICkanApi ckanApi, IContentProvider contentProvider)
        {
            _ckanApi = ckanApi;
            _contentProvider = contentProvider;
        }

        public CkanGroup CreateOrRetrieveGroup(Profile profile)
        {
            var groupId = CkanGroup.GetNewName(profile.UrlKey);
            return GetCkanGroup(groupId, profile);
        }

        public CkanGroup GetExistingGroup(string groupId)
        {
            return GetCkanGroup(groupId, null);
        }

        public CkanGroup UpdateGroupWithProfileProperties(Profile profile, CkanGroup ckanGroup)
        {
            SetProperties(ckanGroup, profile);
            if (profile != null)
            {
                ckanGroup = _ckanApi.UpdateGroup(ckanGroup);
            }
            return ckanGroup;
        }

        private CkanGroup GetCkanGroup(string groupId, Profile profile)
        {
            CkanGroup ckanGroup = null;
            int attemptCount = 0;
            while (ckanGroup == null)
            {
                attemptCount++;
                if (attemptCount > 10)
                {
                    throw new CkanApiException("Repeated exceptions, reached retry limit");
                }
                ckanGroup = CreateOrRetrieveGroup(groupId, profile);
            }
            return ckanGroup;
        }

        private CkanGroup CreateOrRetrieveGroup(string groupId, Profile profile)
        {
            CkanGroup ckanGroup;
            try
            {
                ckanGroup = _ckanApi.GetGroup(groupId);
                if (ckanGroup == null)
                {
                    // Create new group
                    var unsavedCkanGroup = new CkanGroup
                    {
                        Name = groupId,
                    };
                    SetProperties(unsavedCkanGroup, profile);
                    ckanGroup = _ckanApi.CreateGroup(unsavedCkanGroup);
                }
            }
            catch (Exception ex)
            {
                if (ex is CkanTimeoutException || ex is CkanInternalServerException)
                {
                    ckanGroup = null;

                    if (WaitIfResourceUploadFails)
                    {
                        // Wait 10s before continuing
                        Thread.Sleep(1000 * 10);
                    }
                }
                else
                {
                    throw;
                }
            }
            return ckanGroup;
        }

        private void SetProperties(CkanGroup ckanGroup, Profile profile)
        {
            if (profile != null)
            {
                var profileName = profile.Name;
                var description = _contentProvider
                    .GetContentWithHtmlRemoved(profile.Id, ContentKeys.CkanDescription);

                ckanGroup.Title = profileName;
                ckanGroup.Description = description;
            }
        }
    }
}