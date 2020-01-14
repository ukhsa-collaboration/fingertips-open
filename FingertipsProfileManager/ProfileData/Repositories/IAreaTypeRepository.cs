using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;

namespace Fpm.ProfileData.Repositories
{
    public interface IAreaTypeRepository
    {
        IList<AreaType> GetAllAreaTypes();
        AreaType GetAreaType(int areaTypeId);
        IList<AreaTypeComponent> GetAreaTypeComponents(int parentAreaTypeId);
        void SaveAreaType(AreaType areaType);
    }
}