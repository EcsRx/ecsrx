using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Extensions;
using Persistity.Endpoints;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps;
using Persistity.Flow.Steps.Types;

namespace EcsRx.Plugins.Persistence.Builders
{
    public class EcsRxPipelineBuilder
    {
        private readonly IDependencyResolver _resolver;

        public EcsRxPipelineBuilder(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }
        
        public EcsRxPipelineNeedsObjectBuilder StartFromInput()
        { return new EcsRxPipelineNeedsObjectBuilder(_resolver, new List<IPipelineStep>()); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_resolver, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<object, Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_resolver, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom(IReceiveDataEndpoint endpoint)
        { return new EcsRxPipelineNeedsDataBuilder(_resolver, new List<IPipelineStep>{ new ReceiveEndpointStep(endpoint)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom<T>() where T : IReceiveDataEndpoint
        { return new EcsRxPipelineNeedsDataBuilder(_resolver, new List<IPipelineStep>{ new ReceiveEndpointStep(_resolver.Resolve<T>())}); }

        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom(IFlowPipeline flowPipeline, int forkAtStep = -1)
        {
            var stepsToTake = forkAtStep == -1 ? 
                flowPipeline.Steps.ToList() : 
                flowPipeline.Steps.Take(forkAtStep).ToList();
            
            if(stepsToTake.Last() is IReturnsData)
            { throw new ArgumentException("Step being forked returns Data not an Object", nameof(forkAtStep)); }
            
            return new EcsRxPipelineNeedsObjectBuilder(_resolver, stepsToTake);
        }
        
        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom<T>(int forkAtStep = -1) where T : IFlowPipeline
        { return ForkObjectFrom(_resolver.Resolve<T>(), forkAtStep); }
        
        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom<T>(string pipelineName, int forkAtStep = -1) where T : IFlowPipeline
        { return ForkObjectFrom((IFlowPipeline)_resolver.ResolvePipeline(pipelineName), forkAtStep); }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom(IFlowPipeline flowPipeline, int forkAtStep = -1)
        {
            var stepsToTake = forkAtStep == -1 ? 
                flowPipeline.Steps.ToList() : 
                flowPipeline.Steps.Take(forkAtStep).ToList();
            
            if(stepsToTake.Last() is IReturnsObject)
            { throw new ArgumentException("Step being forked returns an Object not Data", nameof(forkAtStep)); }
            
            return new EcsRxPipelineNeedsDataBuilder(_resolver, stepsToTake);
        }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom<T>(int forkAtStep = -1) where T : IFlowPipeline
        { return ForkDataFrom(_resolver.Resolve<T>(), forkAtStep); }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom<T>(string pipelineName, int forkAtStep = -1) where T : IFlowPipeline
        { return ForkDataFrom((IFlowPipeline)_resolver.ResolvePipeline(pipelineName), forkAtStep); }
        
    }
}