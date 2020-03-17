using System.Net.Http;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using LazyData.Json;
using LazyData.Json.Handlers;
using Persistity.Endpoints.Http;
using Persistity.Pipelines;
using Persistity.Pipelines.Builders;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Modules
{
    public class PipelineModule : IDependencyModule
    {
        public const string PipelineName = "SendJson";
        
        public void Setup(IDependencyContainer container)
        {
            // By default only the binary stuff is loaded, but you can load json, yaml, bson etc
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            container.Bind<IJsonSerializer, JsonSerializer>();
            container.Bind<IJsonDeserializer, JsonDeserializer>();
            
            // Register our custom pipeline using the json stuff above
            container.Bind<ISendDataPipeline>(x => 
                x.ToMethod(CreateJsonPostingPipeline).WithName(PipelineName));
        }

        public ISendDataPipeline CreateJsonPostingPipeline(IDependencyContainer container)
        {
            // This pipeline builder lets you make your own pipelines in a fluent way
            var pipelineBuilder = container.Resolve<EcsRxPipelineBuilder>();
            
            // Setup a pipeline that serializes to JSON then sends to postman (which just tells us what we sent)
            return pipelineBuilder
                .SerializeWith<IJsonSerializer>()
                .SendTo(new HttpSendEndpoint("https://postman-echo.com/post", HttpMethod.Post))
                .Build();
        }
    }
}