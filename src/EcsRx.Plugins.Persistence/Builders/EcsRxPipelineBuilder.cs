using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Pipelines.Builders;
using Persistity.Pipelines.Steps;
using Persistity.Pipelines.Steps.Types;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class EcsRxPipelineBuilder
    {
        private readonly IDependencyContainer _container;

        public EcsRxPipelineBuilder(IDependencyContainer container)
        {
            _container = container;
        }
        
        public EcsRxPipelineNeedsObjectBuilder StartFromInput()
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>()); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<object, Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom(IReceiveDataEndpoint endpoint)
        { return new EcsRxPipelineNeedsDataBuilder(_container, new List<IPipelineStep>{ new ReceiveEndpointStep(endpoint)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom<T>() where T : IReceiveDataEndpoint
        { return new EcsRxPipelineNeedsDataBuilder(_container, new List<IPipelineStep>{ new ReceiveEndpointStep(_container.Resolve<T>())}); }

    }
}