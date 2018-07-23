using PholioVisualisation.DataAccess.Repositories;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class AuditProvider
    {
        private readonly CoreDataAuditRepository _repository;
        private readonly FpmUserRepository _userRepository;

        public AuditProvider()
        {
            _repository = new CoreDataAuditRepository();
            _userRepository = new FpmUserRepository();
        }

        public DataChange GetLatestAuditData(int indicatorId)
        {
            var uploadAudit = _repository.GetLatestUploadAuditData(indicatorId);
            var deleteAudit = _repository.GetLatestDeleteAuditData(indicatorId);

            if (uploadAudit == null && deleteAudit == null)
            {
                return null;
            }

            var data = new DataChange();

            if (uploadAudit != null)
            {
                data.LastUploadedAt = uploadAudit.DateCreated;
                data.LastUploadedBy = GetUserName(uploadAudit.UserId);
            }

            if (deleteAudit != null)
            {
                data.LastDeletedAt = deleteAudit.DateDeleted;
                data.LastDeletedBy = GetUserDisplayNameFromUsername(deleteAudit.DeletedBy);
            }

            return data;
        }

        private string GetUserName(int id)
        {
            var user = _userRepository.GetUserById(id);
            var username = user != null ? user.DisplayName : "";
            return username;
        }

        private string GetUserDisplayNameFromUsername(string username)
        {
            var user = _userRepository.GetUserDisplayNameFromUsername(username);
            return user.DisplayName;
        }

    }
}
