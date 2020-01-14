using IndicatorsUI.DataAccess;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Configuration = NHibernate.Cfg.Configuration;

namespace IndicatorsUI.MainUISeleniumTest
{
    public interface IRepositoryContainer
    {
        int ExecuteInsert(string query);
        void ExecuteUpdate(string query);
        IEnumerable<object> ExecuteQuery(string query);
    }

    public class RepositoryContainer : IRepositoryContainer
    {
        private readonly ISessionFactory _sessionFactory;

        private static readonly string LogPath = ConfigurationManager.AppSettings["LoggerTest"];

        public RepositoryContainer(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public RepositoryContainer()
        {
            var cfg = new Configuration();

            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = ConfigurationManager.ConnectionStrings["FingertipsUsersConnectionString"].ConnectionString;
                x.Driver<SqlClientDriver>();
                x.Dialect<MsSql2012Dialect>();
            });

            cfg.AddAssembly(Assembly.GetExecutingAssembly());

            _sessionFactory = cfg.BuildSessionFactory();
        }

        public int ExecuteInsert(string query)
        {
            var results = 0;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var q = session.CreateSQLQuery(query);
                    if (query.Contains("OUTPUT Inserted.ID"))
                    {
                        results = (int)q.UniqueResult();
                    }
                    else
                    {
                        q.UniqueResult();
                    }

                    tx.Commit();
                }
            }

            return results;
        }

        public void ExecuteUpdate(string query)
        {
            try
            {
                using (var session = _sessionFactory.OpenSession())
                {
                    using (var tx = session.BeginTransaction())
                    {
                        var q = session.CreateSQLQuery(query);
                        q.UniqueResult();
                        tx.Commit();
                    }
                }
            }
            catch (Exception)
            {
                var exception = new Exception("The executed sql query failed");
                var logger = new FileLogger(LogPath);
                logger.WriteException(exception);
                throw exception;
            }
        }

        public IEnumerable<object> ExecuteQuery(string query)
        {
            object results;
            using (var session = _sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var q = session.CreateSQLQuery(query);
                    results = q.List<object>();
                    tx.Commit();
                }
            }

            return (IList<object>)results;
        }
    }
}
