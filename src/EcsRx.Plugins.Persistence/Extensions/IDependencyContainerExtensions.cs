using System;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Extensions
{
    public static class IDependencyContainerExtensions
    {
        /// <summary>
        /// Resolves a data pipeline
        /// </summary>
        /// <param name="resolver">The resolver to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The pipeline with the given name</returns>
        public static IPipeline ResolvePipeline(this IDependencyResolver resolver, string name)
        {
            var pipeline = resolver.Resolve<IPipeline>(name);
            
            if(pipeline == null)
            { throw new Exception($"No pipeline registered with the name \"{name}\"");}
            
            return pipeline;
        }
        
        /// <summary>
        /// Builds a pipeline for objects
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="buildFunction">The action to build the pipeline</param>
        public static void BuildPipeline(this IDependencyRegistry registry, string name, Func<EcsRxPipelineBuilder, EcsRxPipelineNeedsObjectBuilder> buildFunction)
        {
            registry.Bind<IPipeline>(builder => builder
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
        public static void BuildPipeline(this IDependencyRegistry registry, string name, Func<EcsRxPipelineBuilder, EcsRxPipelineNeedsDataBuilder> buildFunction)
        {
            registry.Bind<IPipeline>(builder => builder
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