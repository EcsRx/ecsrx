using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using EcsRx.Plugins.Persistence.Modules;
using EcsRx.Plugins.Persistence.Pipelines;
using LazyData.Json;
using LazyData.Json.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Registries;

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
            
            // Create the serializer to serialize everything
            var everythingSerializer = new JsonSerializer(mappingRegistry, primitiveTypeMappings);
            
            // Piggyback off the existing save pipeline helper, which lets you set your format and filename
            return PersistityModule.CreateSavePipeline(container, everythingSerializer, CustomEntityDatabaseFile);
        }
        
        public ILoadEntityDatabasePipeline CreateJsonLoadPipeline(IDependencyContainer container)
        {
            // Manually build deserializer as we want to load everything
            var mappingRegistry = new MappingRegistry(container.Resolve<EverythingTypeMapper>());
            var typeCreator = container.Resolve<ITypeCreator>();
            var primitiveTypeMappings = container.ResolveAll<IJsonPrimitiveHandler>();

            // Create deserializer for everything
            var everythingDeserializer = new JsonDeserializer(mappingRegistry, typeCreator, primitiveTypeMappings);
            
            // Use existing load pipeline helper to customize format and filename
            return PersistityModule.CreateLoadPipeline(container, everythingDeserializer, CustomEntityDatabaseFile);
        }
        
    }
}