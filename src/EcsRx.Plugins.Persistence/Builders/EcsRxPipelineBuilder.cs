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
        private readonly IDependencyContainer _container;

        public EcsRxPipelineBuilder(IDependencyContainer container)
        {
            _container = container;
        }
        
        public EcsRxPipelineNeedsObjectBuilder StartFromInput()
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>()); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsObjectBuilder StartFrom(Func<object, Task<object>> method)
        { return new EcsRxPipelineNeedsObjectBuilder(_container, new List<IPipelineStep>{ new ReceiveMethodStep(method)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom(IReceiveDataEndpoint endpoint)
        { return new EcsRxPipelineNeedsDataBuilder(_container, new List<IPipelineStep>{ new ReceiveEndpointStep(endpoint)}); }
        
        public EcsRxPipelineNeedsDataBuilder StartFrom<T>() where T : IReceiveDataEndpoint
        { return new EcsRxPipelineNeedsDataBuilder(_container, new List<IPipelineStep>{ new ReceiveEndpointStep(_container.Resolve<T>())}); }

        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom(IFlowPipeline flowPipeline, int forkAtStep = -1)
        {
            var stepsToTake = forkAtStep == -1 ? 
                flowPipeline.Steps.ToList() : 
                flowPipeline.Steps.Take(forkAtStep).ToList();
            
            if(stepsToTake.Last() is IReturnsData)
            { throw new ArgumentException("Step being forked returns Data not an Object", nameof(forkAtStep)); }
            
            return new EcsRxPipelineNeedsObjectBuilder(_container, stepsToTake);
        }
        
        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom<T>(int forkAtStep = -1) where T : IFlowPipeline
        { return ForkObjectFrom(_container.Resolve<T>(), forkAtStep); }
        
        public EcsRxPipelineNeedsObjectBuilder ForkObjectFrom<T>(string pipelineName, int forkAtStep = -1) where T : IFlowPipeline
        { return ForkObjectFrom((IFlowPipeline)_container.ResolvePipeline(pipelineName), forkAtStep); }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom(IFlowPipeline flowPipeline, int forkAtStep = -1)
        {
            var stepsToTake = forkAtStep == -1 ? 
                flowPipeline.Steps.ToList() : 
                flowPipeline.Steps.Take(forkAtStep).ToList();
            
            if(stepsToTake.Last() is IReturnsObject)
            { throw new ArgumentException("Step being forked returns an Object not Data", nameof(forkAtStep)); }
            
            return new EcsRxPipelineNeedsDataBuilder(_container, stepsToTake);
        }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom<T>(int forkAtStep = -1) where T : IFlowPipeline
        { return ForkDataFrom(_container.Resolve<T>(), forkAtStep); }
        
        public EcsRxPipelineNeedsDataBuilder ForkDataFrom<T>(string pipelineName, int forkAtStep = -1) where T : IFlowPipeline
        { return ForkDataFrom((IFlowPipeline)_container.ResolvePipeline(pipelineName), forkAtStep); }
        
    }
}