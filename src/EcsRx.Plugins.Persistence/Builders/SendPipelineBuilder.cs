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
    public class SendPipelineBuilder
    {
        private readonly IDependencyContainer _container;
        private readonly ISerializer _serializer;
        private readonly IList<IProcessor> _processors;
        private readonly IList<ITransformer> _transformers;
        private ISendDataEndpoint _sendDataEndpointStep;

        public SendPipelineBuilder(IDependencyContainer container, ISerializer serializer)
        {
            _container = container;
            _serializer = serializer;
            _processors = new List<IProcessor>();
            _transformers = new List<ITransformer>();
        }

        public SendPipelineBuilder ProcessWith<T>() where T : IProcessor
        {
            _processors.Add(_container.Resolve<T>());
            return this;
        }

        public SendPipelineBuilder ProcessWith(IProcessor processor)
        {
            _processors.Add(processor);
            return this;
        }
        
        public SendPipelineBuilder TransformWith<T>() where T : ITransformer
        {
            _transformers.Add(_container.Resolve<T>());
            return this;
        }
        
        public SendPipelineBuilder TransformWith(ITransformer transformer)
        {
            _transformers.Add(transformer);
            return this;
        }
        
        public SendPipelineBuilder SendTo<T>() where T : ISendDataEndpoint
        {
            _sendDataEndpointStep = _container.Resolve<T>();
            return this;
        }

        public SendPipelineBuilder SendTo(ISendDataEndpoint sendDataEndpoint)
        {
            _sendDataEndpointStep = sendDataEndpoint;
            return this;
        }

        public ISendDataPipeline Build()
        {
            return new SendDataPipeline(_serializer, _sendDataEndpointStep, _processors, _transformers);
        }
    }
}