using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Modules;
using EcsRx.Plugins.Persistence.Pipelines;
using LazyData.Json.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;
using LazyJsonSerializer = LazyData.Json.JsonSerializer;
using LazyJsonDeserializer = LazyData.Json.JsonDeserializer;
using Persistity.Serializers.LazyData.Json;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class JsonEntityDatabaseModule : IDependencyModule
    {
        public const string CustomEntityDatabaseFile = "entity-database.json";
        
        public void Setup(IDependencyContainer container)
        {
            // Override our default save pipeline (binary ones) with the json one
            container.Unbind<ISaveEntityDatabasePipeline>();
            container.Bind<ISaveEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateJsonSavePipeline).AsSingleton());
            
            // Override our default load pipeline (binary ones) with the json one
            container.Unbind<ILoadEntityDatabasePipeline>();
            container.Bind<ILoadEntityDatabasePipeline>(builder =>
                builder.ToMethod(CreateJsonLoadPipeline).AsSingleton());
        }

        public ISaveEntityDatabasePipeline CreateJsonSavePipeline(IDependencyContainer container)
        {
            // We manually create our serializer here as we dont want the default behaviour which
            // which would be to only persist things with `[Persist]` and `[PersistData]` attributes
            // we want to persist EVERYTHING
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var primitiveTypeMappings = container.ResolveAll<IJsonPrimitiveHandler>();
            
            // Create the lazy serializer to serialize everything, then wrap it in the persistity one
            var everythingSerializer = new LazyJsonSerializer(mappingRegistry, primitiveTypeMappings);
            var serializer = new JsonSerializer(everythingSerializer);
            
            // Piggyback off the existing save pipeline helper, which lets you set your format and filename
            return PersistityModule.CreateSavePipeline(container, serializer, CustomEntityDatabaseFile);
        }
        
        public ILoadEntityDatabasePipeline CreateJsonLoadPipeline(IDependencyContainer container)
        {
            // Manually build deserializer as we want to load everything
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var typeCreator = container.Resolve<ITypeCreator>();
            var primitiveTypeMappings = container.ResolveAll<IJsonPrimitiveHandler>();

            // Create deserializer for everything
            var everythingDeserializer = new LazyJsonDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            var deserializer = new JsonDeserializer(everythingDeserializer);
            
            // Use existing load pipeline helper to customize format and filename
            return PersistityModule.CreateLoadPipeline(container, deserializer, CustomEntityDatabaseFile);
        }
        
    }
}