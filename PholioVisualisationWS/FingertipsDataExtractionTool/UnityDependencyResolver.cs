using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Unity;
using Unity.Exceptions;
using IDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace FingertipsDataExtractionTool
{
    /// <summary>
    /// The <see cref="UnityDependencyResolver"/>
    ///   class is used to provide dependency resolution support using an <see cref="IUnityContainer"/> instance.
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Stores whether this instance has been disposed.
        /// </summary>
        private bool _isDisposed;

        private IUnityContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityDependencyResolver"/> class.
        /// </summary>
        /// <param name="container">
        /// The container. 
        /// </param>
        public UnityDependencyResolver(IUnityContainer container)
        {
            //Contract.Requires<ArgumentNullException>(container != null);

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            _container = container;
        }

        /// <summary>
        /// Begins the scope.
        /// </summary>
        /// <returns>
        /// A <see cref="IDependencyScope"/> instance. 
        /// </returns>
        public IDependencyScope BeginScope()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            var child = this._container.CreateChildContainer();
            return new UnityDependencyResolver(child);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="serviceType">
        /// Type of the service. 
        /// </param>
        /// <returns>
        /// A <see cref="Object"/> instance. 
        /// </returns>
        public object GetService(Type serviceType)
        {
            try
            {
                return this._container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <param name="serviceType">
        /// Type of the service. 
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable&lt;T&gt;"/> instance. 
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return this._container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. 
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // Free managed resources
                _container.Dispose();
            }

            // Free native resources if there are any
            _isDisposed = true;
        }

        public static implicit operator DependencyResolver(UnityDependencyResolver v)
        {
            throw new NotImplementedException();
        }
    }
}
