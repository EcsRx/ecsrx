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
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            container.Bind<IJsonSerializer, JsonSerializer>();
            container.Bind<IJsonDeserializer, JsonDeserializer>();
            container.Bind<ISendDataPipeline>(x => 
                x.ToMethod(CreateJsonPostingPipeline).WithName(PipelineName));
        }

        public ISendDataPipeline CreateJsonPostingPipeline(IDependencyContainer container)
        {
            var pipelineBuilder = container.Resolve<EcsRxPipelineBuilder>();
            return pipelineBuilder
                .SerializeWith<IJsonSerializer>()
                .SendTo(new HttpSendEndpoint("https://postman-echo.com/post", HttpMethod.Post))
                .Build();
        }
    }
}