using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Pipelines;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Binary.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;
using Persistity.Core.Serialization;
using Persistity.Endpoints.Files;
using Persistity.Serializers.LazyData.Binary;
using LazyBinarySerializer = LazyData.Binary.BinarySerializer;
using LazyBinaryDeserializer = LazyData.Binary.BinaryDeserializer;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class PersistityModule : IDependencyModule
    {
        public static readonly string DefaultEntityDatabaseFile = "entity-database.edb";
        
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<IToEntityDataTransformer, ToEntityDataTransformer>();
            registry.Bind<IToEntityCollectionDataTransformer, ToEntityCollectionDataTransformer>();
            registry.Bind<IToEntityDatabaseDataTransformer, ToEntityDatabaseDataTransformer>();
            registry.Bind<IFromEntityDataTransformer, FromEntityDataTransformer>();
            registry.Bind<IFromEntityCollectionDataTransformer, FromEntityCollectionDataTransformer>();
            registry.Bind<IFromEntityDatabaseDataTransformer, FromEntityDatabaseDataTransformer>();
            registry.Bind<EcsRxPipelineBuilder>(builder => builder.ToMethod(x =>
                new EcsRxPipelineBuilder(x)).AsSingleton());

            // These are defaults, you can override these in your own app/plugin
            registry.Bind<ISaveEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateDefaultSavePipeline).AsSingleton());
            
            registry.Bind<ILoadEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateDefaultLoadPipeline).AsSingleton());
        }

        public ISaveEntityDatabasePipeline CreateDefaultSavePipeline(IDependencyResolver resolver)
        {
            var mappingRegistry = new MappingRegistry(resolver.Resolve<EverythingTypeMapper>());
            var primitiveTypeMappings = resolver.ResolveAll<IBinaryPrimitiveHandler>();
            var everythingSerializer = new LazyBinarySerializer(mappingRegistry, primitiveTypeMappings);
            var persistitySerializer = new BinarySerializer(everythingSerializer);
            return CreateSavePipeline(resolver, persistitySerializer, DefaultEntityDatabaseFile);
        }

        public ILoadEntityDatabasePipeline CreateDefaultLoadPipeline(IDependencyResolver resolver)
        {
            var mappingRegistry = new MappingRegistry(resolver.Resolve<EverythingTypeMapper>());
            var typeCreator = resolver.Resolve<ITypeCreator>();
            var primitiveTypeMappings = resolver.ResolveAll<IBinaryPrimitiveHandler>();
            var everythingDeserializer = new LazyBinaryDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            var persistityDeserializer = new BinaryDeserializer(everythingDeserializer);
            return CreateLoadPipeline(resolver, persistityDeserializer, DefaultEntityDatabaseFile);
        }
        
        /// <summary>
        /// This can be re-used if you want to just swap over to use JSON or XML etc
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="filename">The filename to use</param>
        /// <returns>The save database pipeline with config provided</returns>
        public static ISaveEntityDatabasePipeline CreateSavePipeline(IDependencyResolver resolver, 
            ISerializer serializer, string filename)
        {
            return new DefaultSaveEntityDatabasePipeline(
                resolver.Resolve<EcsRxPipelineBuilder>(), serializer, 
                resolver.Resolve<IToEntityDatabaseDataTransformer>(), 
                new FileEndpoint(filename));
        }

        /// <summary>
        /// This can be re-used if you want to just swap over to use JSON or XML etc
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <param name="deserializer">The deserializer to use</param>
        /// <param name="filename">The filename to use</param>
        /// <returns>The save database pipeline with config provided</returns>
        public static ILoadEntityDatabasePipeline CreateLoadPipeline(IDependencyResolver resolver,
            IDeserializer deserializer, string filename)
        {
            return new DefaultLoadEntityDatabasePipeline(
                resolver.Resolve<EcsRxPipelineBuilder>(), deserializer,
                resolver.Resolve<IFromEntityDatabaseDataTransformer>(), 
                new FileEndpoint(filename));
        }
    }
}