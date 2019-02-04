using System;
using NHibernate;
using NHibernate.Criterion;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataAccess.Repositories
{
    public class FpmDocumentRepository : RepositoryBase
    {
        public FpmDocumentRepository() : this(NHibernateSessionFactory.GetSession())
        {
        }

        public FpmDocumentRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public void PublishDocument(FpmDocument document)
        {
            ITransaction transaction = null;

            try
            {
                // Begin transaction
                transaction = CurrentSession.BeginTransaction();

                var documentInDb = CurrentSession.CreateCriteria<FpmDocument>()
                    .Add(Restrictions.Eq("ProfileId", document.ProfileId))
                    .Add(Restrictions.Eq("FileName", document.FileName))
                    .UniqueResult<FpmDocument>();

                if (documentInDb == null)
                {
                    CurrentSession.GetNamedQuery("Insert_FpmDocument")
                        .SetParameter("ProfileId", document.ProfileId)
                        .SetParameter("FileName", document.FileName)
                        .SetParameter("FileData", document.FileData, NHibernate.Type.TypeFactory.GetBinaryType((document.FileData).Length))
                        .SetParameter("UploadedOn", DateTime.Now)
                        .SetParameter("UploadedBy", document.UploadedBy)
                        .ExecuteUpdate();
                }
                else
                {
                    if (documentInDb.ProfileId == document.ProfileId && documentInDb.FileName == document.FileName)
                    {
                        IQuery q = CurrentSession.CreateQuery("update FpmDocument f set f.FileData = :FileData where f.ProfileId = :ProfileId and f.FileName = :FileName");
                        q.SetParameter("FileData", document.FileData,
                            NHibernate.Type.TypeFactory.GetBinaryType((document.FileData).Length));
                        q.SetParameter("ProfileId", document.ProfileId);
                        q.SetParameter("FileName", document.FileName);
                        q.ExecuteUpdate();
                    }
                    else
                    {
                        throw new Exception(string.Format("No match found for the profile id {0} and the document name {1}", document.ProfileId, document.FileName));
                    }
                }

                // All went well, commit the transaction
                transaction.Commit();
            }
            catch (Exception)
            {
                // Something wrong, rollback the transaction
                if (transaction != null && transaction.WasRolledBack == false)
                {
                    transaction.Rollback();
                }

                // Throw the exception
                throw;
            }
        }
    }
}
