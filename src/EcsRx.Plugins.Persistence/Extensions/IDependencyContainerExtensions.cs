using System;
using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Extensions
{
    public static class IDependencyContainerExtensions
    {
        /// <summary>
        /// Resolves a data pipeline
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The pipeline with the given name</returns>
        public static IPipeline ResolvePipeline(this IDependencyContainer container, string name)
        {
            if(!container.HasBinding<IPipeline>(name))
            { throw new Exception($"No pipeline registered with the name \"{name}\"");}
            
            return container.Resolve<IPipeline>(name);
        }
        
        /// <summary>
        /// Builds a pipeline for objects
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="buildFunction">The action to build the pipeline</param>
        public static void BuildPipeline(this IDependencyContainer container, string name, Func<EcsRxPipelineBuilder, EcsRxPipelineNeedsObjectBuilder> buildFunction)
        {
            container.Bind<IPipeline>(builder => builder
                .ToMethod(x =>
                {
                    var pipelineBuilder = x.Resolve<EcsRxPipelineBuilder>();
                    return buildFunction(pipelineBuilder).Build();
                })
                .WithName(name)
                .AsSingleton());
        }

        /// <summary>
        /// Builds a pipeline for data
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="buildFunction">The action to build the pipeline</param>
        public static void BuildPipeline(this IDependencyContainer container, string name, Func<EcsRxPipelineBuilder, EcsRxPipelineNeedsDataBuilder> buildFunction)
        {
            container.Bind<IPipeline>(builder => builder
                .ToMethod(x =>
                {
                    var pipelineBuilder = x.Resolve<EcsRxPipelineBuilder>();
                    return buildFunction(pipelineBuilder).Build();
                })
                .WithName(name)
                .AsSingleton());
        }
    }
}