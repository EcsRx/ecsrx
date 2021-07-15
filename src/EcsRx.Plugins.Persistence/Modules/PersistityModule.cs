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
        
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IToEntityDataTransformer, ToEntityDataTransformer>();
            container.Bind<IToEntityCollectionDataTransformer, ToEntityCollectionDataTransformer>();
            container.Bind<IToEntityDatabaseDataTransformer, ToEntityDatabaseDataTransformer>();
            container.Bind<IFromEntityDataTransformer, FromEntityDataTransformer>();
            container.Bind<IFromEntityCollectionDataTransformer, FromEntityCollectionDataTransformer>();
            container.Bind<IFromEntityDatabaseDataTransformer, FromEntityDatabaseDataTransformer>();
            container.Bind<EcsRxPipelineBuilder>(builder => builder.ToMethod(x =>
                new EcsRxPipelineBuilder(x)).AsSingleton());

            // These are defaults, you can override these in your own app/plugin
            container.Bind<ISaveEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateDefaultSavePipeline).AsSingleton());
            
            container.Bind<ILoadEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateDefaultLoadPipeline).AsSingleton());
        }

        public ISaveEntityDatabasePipeline CreateDefaultSavePipeline(IDependencyContainer container)
        {
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var primitiveTypeMappings = container.ResolveAll<IBinaryPrimitiveHandler>();
            var everythingSerializer = new LazyBinarySerializer(mappingRegistry, primitiveTypeMappings);
            var persistitySerializer = new BinarySerializer(everythingSerializer);
            return CreateSavePipeline(container, persistitySerializer, DefaultEntityDatabaseFile);
        }

        public ILoadEntityDatabasePipeline CreateDefaultLoadPipeline(IDependencyContainer container)
        {
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var typeCreator = container.Resolve<ITypeCreator>();
            var primitiveTypeMappings = container.ResolveAll<IBinaryPrimitiveHandler>();
            var everythingDeserializer = new LazyBinaryDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            var persistityDeserializer = new BinaryDeserializer(everythingDeserializer);
            return CreateLoadPipeline(container, persistityDeserializer, DefaultEntityDatabaseFile);
        }
        
        /// <summary>
        /// This can be re-used if you want to just swap over to use JSON or XML etc
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="filename">The filename to use</param>
        /// <returns>The save database pipeline with config provided</returns>
        public static ISaveEntityDatabasePipeline CreateSavePipeline(IDependencyContainer container, 
            ISerializer serializer, string filename)
        {
            return new DefaultSaveEntityDatabasePipeline(
                container.Resolve<EcsRxPipelineBuilder>(), serializer, 
                container.Resolve<IToEntityDatabaseDataTransformer>(), 
                new FileEndpoint(filename));
        }

        /// <summary>
        /// This can be re-used if you want to just swap over to use JSON or XML etc
        /// </summary>
        /// <param name="container">The dependency container</param>
        /// <param name="deserializer">The deserializer to use</param>
        /// <param name="filename">The filename to use</param>
        /// <returns>The save database pipeline with config provided</returns>
        public static ILoadEntityDatabasePipeline CreateLoadPipeline(IDependencyContainer container,
            IDeserializer deserializer, string filename)
        {
            return new DefaultLoadEntityDatabasePipeline(
                container.Resolve<EcsRxPipelineBuilder>(), deserializer,
                container.Resolve<IFromEntityDatabaseDataTransformer>(), 
                new FileEndpoint(filename));
        }
    }
}