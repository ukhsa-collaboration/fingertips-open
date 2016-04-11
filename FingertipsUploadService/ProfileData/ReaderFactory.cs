using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;

namespace FingertipsUploadService.ProfileData
{
    /// <summary>
    /// An implementation of IDataAccessServiceFactory that uses NHibernate to persist data
    /// </summary>
    public sealed class ReaderFactory : IDisposable
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
        public ReaderFactory(IEnumerable<string> assemblyNames)
        {
            Exception exception = null;

            // If SessionFactory build fails then retry
            for (int tryNumber = 1; tryNumber <= 3; tryNumber++)
            {
                try
                {
                    Configuration config = new Configuration();

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

        public static ProfilesReader GetProfilesReader()
        {
            ProfilesReader service = new ProfilesReader(GetSessionFactory());
            service.OpenSession();
            return service;
        }

        private static ISessionFactory GetSessionFactory()
        {
            if (staticSessionFactory == null)
            {
                List<string> assemblyNames = new List<string>();
                assemblyNames.Add("FingertipsUploadService.ProfileData");
                staticSessionFactory = new ReaderFactory(assemblyNames).sessionFactory;
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
