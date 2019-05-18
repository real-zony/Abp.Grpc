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
}