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
        
        public SendPipelineBuilder SerializeWith(ISerializer serializer)
        { return new SendPipelineBuilder(_container, serializer); }

        public SendPipelineBuilder SerializeWith<T>() where T : ISerializer
        { return SerializeWith(_container.Resolve<T>()); }
        
        public ReceivePipelineBuilder RecieveFrom(IReceiveDataEndpoint endpoint)
        { return new ReceivePipelineBuilder(_container, endpoint);  }
        
        public ReceivePipelineBuilder RecieveFrom<T>() where T : IReceiveDataEndpoint
        { return RecieveFrom(_container.Resolve<T>());  }
    }
}