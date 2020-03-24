using System.Collections.Generic;
using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Pipelines.Steps.Types;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityDatabasePipeline : EcsRxBuiltPipeline, ISaveEntityDatabasePipeline
    {
        public ISerializer Serializer { get; }
        public IToEntityDatabaseDataTransformer DataTransformer { get; }
        public ISendDataEndpoint Endpoint { get; }

        public DefaultSaveEntityDatabasePipeline(EcsRxPipelineBuilder pipelineBuilder, ISerializer serializer, IToEntityDatabaseDataTransformer dataTransformer, ISendDataEndpoint endpoint) : base(pipelineBuilder)
        {
            Serializer = serializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;
        }

        public Task Execute(IEntityDatabase entityDatabase)
        { return Execute(entityDatabase, null); }
        
        protected override IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder
                .StartFromInput()
                .TransformWith(DataTransformer)
                .SerializeWith(Serializer)
                .ThenSendTo(Endpoint)
                .BuildSteps();
        }
    }
}