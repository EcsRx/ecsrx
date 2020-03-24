using System.Collections.Generic;
using EcsRx.Plugins.Persistence.Builders;
using Persistity.Pipelines;
using Persistity.Pipelines.Steps.Types;

namespace EcsRx.Plugins.Persistence.Pipelines
{
    public abstract class EcsRxBuiltPipeline : FlowPipeline
    {
        public EcsRxBuiltPipeline(EcsRxPipelineBuilder pipelineBuilder)
        {
            Steps = BuildSteps(pipelineBuilder);
        }

        protected abstract IEnumerable<IPipelineStep> BuildSteps(EcsRxPipelineBuilder builder);
    }
}