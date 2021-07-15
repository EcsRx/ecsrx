using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using Persistity.Core;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps;
using Persistity.Flow.Steps.Types;
using Persistity.Processors;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class EcsRxPipelineNeedsDataBuilder
    {
        private readonly IDependencyContainer _container;
        private readonly List<IPipelineStep> _steps;

        public EcsRxPipelineNeedsDataBuilder(IDependencyContainer container, List<IPipelineStep> steps)
        {
            _container = container;
            _steps = steps;
        }

        public EcsRxPipelineNeedsObjectBuilder DeserializeWith(IDeserializer deserializer, Type type)
        {
            _steps.Add(new DeserializeStep(deserializer, type));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsObjectBuilder DeserializeWith<T>() where T : IDeserializer
        {
            _steps.Add(new DeserializeStep(_container.Resolve<T>(), typeof(T)));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsDataBuilder ProcessWith(IProcessor processor)
        {
            _steps.Add(new ProcessStep(processor));
            return this;
        }
        
        public EcsRxPipelineNeedsDataBuilder ProcessWith<T>() where T : IProcessor
        {
            _steps.Add(new ProcessStep(_container.Resolve<T>()));
            return this;
        }
        
        public EcsRxPipelineNeedsObjectBuilder ThenSendTo(ISendDataEndpoint endpoint)
        {
            _steps.Add(new SendEndpointStep(endpoint));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsObjectBuilder ThenSendTo<T>() where T : ISendDataEndpoint
        {
            _steps.Add(new SendEndpointStep(_container.Resolve<T>()));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsObjectBuilder ThenInvoke(Func<DataObject, object, Task<object>> method)
        {
            _steps.Add(new SendDataToObjectMethodStep(method));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsObjectBuilder ThenInvoke(Func<DataObject, Task<object>> method)
        {
            _steps.Add(new SendDataToObjectMethodStep(method));
            return new EcsRxPipelineNeedsObjectBuilder(_container, _steps);
        }
        
        public EcsRxPipelineNeedsDataBuilder ThenInvoke(Func<DataObject, object, Task<DataObject>> method)
        {
            _steps.Add(new SendDataToDataMethodStep(method));
            return this;
        }
        
        public EcsRxPipelineNeedsDataBuilder ThenInvoke(Func<DataObject, Task<DataObject>> method)
        {
            _steps.Add(new SendDataToDataMethodStep(method));
            return this;
        }
        
        public IEnumerable<IPipelineStep> BuildSteps()
        { return _steps; }
        
        public IFlowPipeline Build()
        { return new DefaultPipeline(_steps); }
    }
}