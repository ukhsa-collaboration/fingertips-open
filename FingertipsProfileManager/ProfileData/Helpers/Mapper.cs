using Fpm.ProfileData.Entities.Core;
using Fpm.Upload;

namespace Fpm.ProfileData.Helpers
{
    public static class Mapper
    {
        public static CoreDataSet ToCoreDataSet(this UploadDataModel uploadDataModel)
        {
            var coreDataSet = new CoreDataSet();
            AutoMapper.Mapper.Map(uploadDataModel, coreDataSet);
            return coreDataSet;
        }


        public static CoreDataSetArchive ToCoreDataSetArchive(this CoreDataSet coreDataSet)
        {
            var coreDataSetArchive = new CoreDataSetArchive();
            AutoMapper.Mapper.Map(coreDataSet, coreDataSetArchive);
            return coreDataSetArchive;
        }

    }
}
