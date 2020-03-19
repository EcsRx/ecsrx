using System;
using EcsRx.Collections;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using Persistity.Pipelines;

namespace EcsRx.Plugins.Persistence.Extensions
{
    public static class IDependencyContainerExtensions
    {
        /// <summary>
        /// Resolves a send data pipeline
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The pipeline with the given name</returns>
        public static ISendDataPipeline ResolveSendPipeline(this IDependencyContainer container, string name)
        { return container.Resolve<ISendDataPipeline>(name); }
        
        /// <summary>
        /// Resolves a send data pipeline
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <returns>The pipeline with the given name</returns>
        public static IReceiveDataPipeline ResolveReceivePipeline(this IDependencyContainer container, string name)
        { return container.Resolve<IReceiveDataPipeline>(name); }
        
        /// <summary>
        /// Builds a pipeline for sending data
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="buildFunction">The action to build the pipeline</param>
        public static void BuildPipeline(this IDependencyContainer container, string name, Func<EcsRxPipelineBuilder, SendPipelineBuilder> buildFunction)
        {
            container.Bind<ISendDataPipeline>(builder => builder
                .ToMethod(x =>
                {
                    var pipelineBuilder = x.Resolve<EcsRxPipelineBuilder>();
                    return buildFunction(pipelineBuilder).Build();
                })
                .WithName(name)
                .AsSingleton());
        }

        /// <summary>
        /// Builds a pipeline for receiving data
        /// </summary>
        /// <param name="container">The container to action on</param>
        /// <param name="name">The name of the pipeline</param>
        /// <param name="buildFunction">The action to build the pipeline</param>
        public static void BuildPipeline(this IDependencyContainer container, string name, Func<EcsRxPipelineBuilder, ReceivePipelineBuilder> buildFunction)
        {
            container.Bind<IReceiveDataPipeline>(builder => builder
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