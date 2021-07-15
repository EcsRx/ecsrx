using System.Collections.Generic;
using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Transformers;
using Persistity.Core.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultSaveEntityDatabasePipeline : FlowPipeline, ISaveEntityDatabasePipeline
    {
        public ISerializer Serializer { get; }
        public IToEntityDatabaseDataTransformer DataTransformer { get; }
        public ISendDataEndpoint Endpoint { get; }

        public DefaultSaveEntityDatabasePipeline(EcsRxPipelineBuilder pipelineBuilder, ISerializer serializer, IToEntityDatabaseDataTransformer dataTransformer, ISendDataEndpoint endpoint)
        {
            Serializer = serializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public Task Execute(IEntityDatabase entityDatabase)
        { return Execute(entityDatabase, null); }
        
        protected IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
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