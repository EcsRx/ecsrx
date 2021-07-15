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
        public void Setup(IDependencyContainer container)
        {
            // By default only the binary stuff is loaded, but you can load json, yaml, bson etc
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            container.Bind<LazyJsonSerializer>();
            container.Bind<LazyJsonDeserializer>();
            container.Bind<JsonSerializer>();
            container.Bind<JsonDeserializer>();
            
            // Register our custom pipeline using the json stuff above
            container.Bind<PostJsonHttpPipeline>();
        }
    }
}