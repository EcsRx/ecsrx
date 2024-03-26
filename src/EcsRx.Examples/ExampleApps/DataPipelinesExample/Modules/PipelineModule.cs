using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Examples.ExampleApps.DataPipelinesExample.Pipelines;
using LazyJsonDeserializer = LazyData.Json.JsonDeserializer;
using LazyJsonSerializer = LazyData.Json.JsonSerializer;
using LazyData.Json.Handlers;
using Persistity.Serializers.LazyData.Json;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Modules
{
    public class PipelineModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            // By default only the binary stuff is loaded, but you can load json, yaml, bson etc
            registry.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            registry.Bind<LazyJsonSerializer>();
            registry.Bind<LazyJsonDeserializer>();
            registry.Bind<JsonSerializer>();
            registry.Bind<JsonDeserializer>();
            
            // Register our custom pipeline using the json stuff above
            registry.Bind<PostJsonHttpPipeline>();
        }
    }
}