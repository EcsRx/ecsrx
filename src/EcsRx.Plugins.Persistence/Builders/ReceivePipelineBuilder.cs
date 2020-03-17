using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Pipelines;
using Persistity.Processors;
using Persistity.Transformers;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class ReceivePipelineBuilder
    {
        private readonly IDependencyContainer _container;
        private readonly IReceiveDataEndpoint _receiveDataEndpointStep;
        private readonly IList<IProcessor> _processors;
        private readonly IList<ITransformer> _transformers;
        private IDeserializer _deserializer;

        public ReceivePipelineBuilder(IDependencyContainer container, IReceiveDataEndpoint receiveDataEndpointStep)
        {
            _container = container;
            _receiveDataEndpointStep = receiveDataEndpointStep;
            _processors = new List<IProcessor>();
            _transformers = new List<ITransformer>();
        }

        public ReceivePipelineBuilder ProcessWith(IProcessor processor)
        {
            _processors.Add(processor);
            return this;
        }

        public ReceivePipelineBuilder ProcessWith<T>() where T : IProcessor
        {
            _processors.Add(_container.Resolve<T>());
            return this;
        }

        public ReceivePipelineBuilder TransformWith(ITransformer transformer)
        {
            _transformers.Add(transformer);
            return this;
        }

        public ReceivePipelineBuilder TransformWith<T>() where T : ITransformer
        {
            _transformers.Add(_container.Resolve<T>());
            return this;
        }

        public ReceivePipelineBuilder DeserializeWith(IDeserializer deserializer)
        {
            _deserializer = deserializer;
            return this;
        }
        
        public ReceivePipelineBuilder DeserializeWith<T>() where T : IDeserializer
        {
            _deserializer = _container.Resolve<T>();
            return this;
        }

        public IReceiveDataPipeline Build()
        {
            return new ReceiveDataPipeline(_deserializer, _receiveDataEndpointStep, _processors, _transformers);
        }
    }
}