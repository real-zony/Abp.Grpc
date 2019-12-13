using System;
using Abp.Dependency;
using MagicOnion.Server;

namespace Abp.Grpc.Server.DependencyInject
{
    public class CasteWindsorServiceLocatorBridge : IServiceLocator
    {
        private readonly IIocManager _iocManager;

        public CasteWindsorServiceLocatorBridge(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        public T GetService<T>()
        {
            return _iocManager.Resolve<T>();
        }

        public IServiceLocatorScope CreateScope()
        {
            return new ServiceLocatorScope(_iocManager.CreateScope());
        }

        public void Register<T>()
        {
            if (!_iocManager.IsRegistered(typeof(T)))
            {
                _iocManager.Register(typeof(T));
            }
        }

        public void Register<T>(T singleton)
        {
            _iocManager.Register(typeof(T));
        }
    }
    
    
    class ServiceLocatorScope : IServiceLocatorScope,IServiceLocator, IServiceProvider
    {
        private readonly IScopedIocResolver _scopedIocResolver;
        
        public ServiceLocatorScope(IScopedIocResolver scopedIocResolver)
        {
            _scopedIocResolver = scopedIocResolver;
        }
        
        public void Dispose()
        {
            _scopedIocResolver.Dispose();
        }

        public IServiceLocator ServiceLocator { get; }
        public T GetService<T>()
        {
            return _scopedIocResolver.Resolve<T>();
        }

        public IServiceLocatorScope CreateScope()
        {
            return new ServiceLocatorScope(_scopedIocResolver.CreateScope());
        }

        public object GetService(Type serviceType)
        {
            return _scopedIocResolver.Resolve(serviceType);
        }
    }
}