using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;

namespace Fpm.ProfileData
{
    public sealed class NHibernateSessionFactory : IDisposable
    {
        /// <summary>
        /// The session factory (very expensive to construct)
        /// </summary>
        private ISessionFactory sessionFactory;

        private static ISessionFactory staticSessionFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assemblyNames">The names of the assemblies containing object-relational mapping information</param>
        /// <exception cref="Exception">Thrown if an error occurred while constructing the factory</exception>
        public NHibernateSessionFactory(IEnumerable<string> assemblyNames)
        {
            Exception exception = null;

            // If SessionFactory build fails then retry
            for (int tryNumber = 1; tryNumber <= 3; tryNumber++)
            {
                try
                {
                    var config = new Configuration();

                    // Add assemblies containing mapping definitions
                    foreach (string assemblyName in assemblyNames)
                    {
                        config.AddAssembly(assemblyName);
                    }

                    config.Configure();
                    sessionFactory = config.BuildSessionFactory();

                    // SessionFactory built successfully
                    exception = null;
                    break;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            if (exception != null)
            {
                throw new Exception("Could not construct ReaderFactory instance", exception);
            }
        }


        public static ISessionFactory GetSession()
        {
            if (staticSessionFactory == null)
            {
                var assemblyNames = new List<string>()
                {
                    "Fpm.ProfileData"
                };
                staticSessionFactory = new NHibernateSessionFactory(assemblyNames).sessionFactory;
            }

            return staticSessionFactory;
        }

        /// <summary>
        /// Disposes of the resources held by the factory
        /// </summary>
        public void Dispose()
        {
            if (sessionFactory != null)
            {
                sessionFactory.Dispose();
                sessionFactory = null;
            }
        }
    }
}