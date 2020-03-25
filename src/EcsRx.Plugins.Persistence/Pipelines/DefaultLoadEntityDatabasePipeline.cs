using System.Collections.Generic;
using System.Threading.Tasks;
using EcsRx.Collections.Database;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Serialization;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public class DefaultLoadEntityDatabasePipeline : FlowPipeline, ILoadEntityDatabasePipeline
    {
        public IDeserializer Deserializer { get; }
        public IFromEntityDatabaseDataTransformer DataTransformer { get; }
        public IReceiveDataEndpoint Endpoint { get; }

        public DefaultLoadEntityDatabasePipeline(EcsRxPipelineBuilder pipelineBuilder, IDeserializer deserializer, IFromEntityDatabaseDataTransformer dataTransformer, IReceiveDataEndpoint endpoint)
        {
            Deserializer = deserializer;
            DataTransformer = dataTransformer;
            Endpoint = endpoint;

            Steps = BuildSteps(pipelineBuilder);
        }

        public async Task<IEntityDatabase> Execute()
        { return (IEntityDatabase) await Execute(null); }

        protected IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder
                .StartFrom(Endpoint)
                .DeserializeWith(Deserializer)
                .TransformWith(DataTransformer)
                .BuildSteps();
        }
    }
}