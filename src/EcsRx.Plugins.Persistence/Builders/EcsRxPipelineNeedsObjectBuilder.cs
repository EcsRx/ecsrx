using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps;
using Persistity.Flow.Steps.Types;
using Persistity.Transformers;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class EcsRxPipelineNeedsObjectBuilder
    {
        private readonly IDependencyContainer _container;
        private List<IPipelineStep> _steps;
        
        public EcsRxPipelineNeedsObjectBuilder(IDependencyContainer container, List<IPipelineStep> steps)
        {
            _container = container;
            _steps = steps;
        }

        public EcsRxPipelineNeedsObjectBuilder(List<IPipelineStep> steps, IDependencyContainer container)
        {
            _steps = steps;
            _container = container;
        }

        public EcsRxPipelineNeedsObjectBuilder TransformWith(ITransformer transformer)
        {
            _steps.Add(new TransformStep(transformer));
            return this;
        }
        
        public EcsRxPipelineNeedsObjectBuilder TransformWith<T>() where T : ITransformer
        {
            _steps.Add(new TransformStep(_container.Resolve<T>()));
            return this;
        }
    
        public EcsRxPipelineNeedsObjectBuilder ThenInvoke(Func<object, object, Task<object>> method)
        {
            _steps.Add(new SendMethodStep(method));
            return this;
        }
    
        public EcsRxPipelineNeedsObjectBuilder ThenInvoke(Func<object, Task<object>> method)
        {
            _steps.Add(new SendMethodStep(method));
            return this;
        }
    
        public EcsRxPipelineNeedsDataBuilder SerializeWith(ISerializer serializer, bool persistType = true)
        {
            _steps.Add(new SerializeStep(serializer, persistType));
            return new EcsRxPipelineNeedsDataBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsDataBuilder SerializeWith<T>(bool persistType = true) where T : ISerializer
        {
            _steps.Add(new SerializeStep(_container.Resolve<T>(), persistType));
            return new EcsRxPipelineNeedsDataBuilder(_container, _steps);
        }
    
        public EcsRxPipelineNeedsDataBuilder ThenReceiveFrom(IReceiveDataEndpoint endpoint)
        {
            _steps.Add(new ReceiveEndpointStep(endpoint));
            return new EcsRxPipelineNeedsDataBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsDataBuilder ThenReceiveFrom<T>() where T : IReceiveDataEndpoint
        {
            _steps.Add(new ReceiveEndpointStep(_container.Resolve<T>()));
            return new EcsRxPipelineNeedsDataBuilder(_container, _steps);
        }

        public EcsRxPipelineNeedsObjectBuilder ThenReceiveFrom(Func<Task<object>> method)
        {
            _steps.Add(new ReceiveMethodStep(method));
            return this;
        }

        public EcsRxPipelineNeedsObjectBuilder ThenReceiveFrom(Func<object, Task<object>> method)
        {
            _steps.Add(new ReceiveMethodStep(method));
            return this;
        }
        
        public IEnumerable<IPipelineStep> BuildSteps()
        { return _steps; }
    
        public IFlowPipeline Build()
        { return new DefaultPipeline(_steps); }
    }
}