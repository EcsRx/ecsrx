using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Pipelines;
using LazyData.Json;
using LazyData.Json.Handlers;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Modules
{
    public class PipelineModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            // By default only the binary stuff is loaded, but you can load json, yaml, bson etc
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            container.Bind<IJsonSerializer, JsonSerializer>();
            container.Bind<IJsonDeserializer, JsonDeserializer>();
            
            // Register our custom pipeline using the json stuff above
            container.Bind<PostJsonHttpPipeline>();
        }
    }
}