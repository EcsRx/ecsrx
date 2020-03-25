using System.Collections.Generic;
using System.Net.Http;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Pipelines;
using LazyData.Json;
using Persistity.Endpoints.Http;
using Persistity.Flow.Steps.Types;

namespace EcsRx.Examples.ExampleApps.DataPipelinesExample.Pipelines
{
    public class PostJsonHttpPipeline : EcsRxBuiltPipeline
    {
        public PostJsonHttpPipeline(EcsRxPipelineBuilder pipelineBuilder) : base(pipelineBuilder)
        { }

        protected override IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder)
        {
            return builder.StartFromInput()
                .SerializeWith<IJsonSerializer>(false)
                .ThenSendTo(new HttpSendEndpoint("https://postman-echo.com/post", HttpMethod.Post))
                .BuildSteps();
        }
    }
}