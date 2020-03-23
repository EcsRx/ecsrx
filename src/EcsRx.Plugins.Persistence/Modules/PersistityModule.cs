using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Builders;
using EcsRx.Plugins.Persistence.Pipelines;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Binary;
using LazyData.Binary.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;
using LazyData.Serialization;
using Persistity.Endpoints.Files;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class PersistityModule : IDependencyModule
    {
        public const string DefaultEntityDatabaseFile = "entity-database.edb";
        
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IToEntityTransformer, ToEntityTransformer>();
            container.Bind<IToEntityCollectionTransformer, ToEntityCollectionTransformer>();
            container.Bind<IToEntityDatabaseTransformer, ToEntityDatabaseTransformer>();
            container.Bind<IFromEntityTransformer, FromEntityTransformer>();
            container.Bind<IFromEntityCollectionTransformer, FromEntityCollectionTransformer>();
            container.Bind<IFromEntityDatabaseTransformer, FromEntityDatabaseTransformer>();
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
            var everythingSerializer = new BinarySerializer(mappingRegistry, primitiveTypeMappings);
            return CreateSavePipeline(container, everythingSerializer, DefaultEntityDatabaseFile);
        }

        public ILoadEntityDatabasePipeline CreateDefaultLoadPipeline(IDependencyContainer container)
        {
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var typeCreator = container.Resolve<ITypeCreator>();
            var primitiveTypeMappings = container.ResolveAll<IBinaryPrimitiveHandler>();
            var everythingDeserializer = new BinaryDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            return CreateLoadPipeline(container, everythingDeserializer, DefaultEntityDatabaseFile);
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
            return new DefaultSaveEntityDatabasePipeline(serializer, 
                container.Resolve<IToEntityDatabaseTransformer>(), 
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
            return new DefaultLoadEntityDatabasePipeline(deserializer,
                container.Resolve<IFromEntityDatabaseTransformer>(), 
                new FileEndpoint(filename));
        }
    }
}