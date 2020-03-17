using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Serialization;
using Persistity.Endpoints;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class EcsRxPipelineBuilder
    {
        private readonly IDependencyContainer _container;

        public EcsRxPipelineBuilder(IDependencyContainer container)
        {
            _container = container;
        }

        public SendPipelineBuilder SerializeWith<T>() where T : ISerializer
        { return new SendPipelineBuilder(_container, _container.Resolve<T>()); }

        public ReceivePipelineBuilder RecieveFrom<T>() where T : IReceiveDataEndpoint
        { return new ReceivePipelineBuilder(_container, _container.Resolve<T>());  }
    }
}