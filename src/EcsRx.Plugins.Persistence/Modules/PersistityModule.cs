using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Plugins.Persistence.Transformers;
using LazyData.Binary;
using LazyData.Binary.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Registries;
using Persistity.Endpoints.Files;
using Persistity.Pipelines;
using Persistity.Pipelines.Builders;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class PersistityModule : IDependencyModule
    {
        public const string DefaultEntityDatabaseFile = "entity-database.edb";
        
        public void Setup(IDependencyContainer container)
        {
            container.Bind<IEntityTransformer, EntityTransformer>();
            container.Bind<IEntityCollectionTransformer, EntityCollectionTransformer>();
            container.Bind<IEntityDatabaseTransformer, EntityDatabaseTransformer>();

            // These are defaults, you can override these in your own app
            container.Bind<ISendDataPipeline>(builder =>
                builder.ToMethod(x =>
                {
                    var mappingRegistry = new MappingRegistry(x.Resolve<EverythingTypeMapper>());
                    var primitiveTypeMappings = x.ResolveAll<IBinaryPrimitiveHandler>();
                    var everythingSerializer = new BinarySerializer(mappingRegistry, primitiveTypeMappings);
                    return new PipelineBuilder()
                        .SerializeWith(everythingSerializer)
                        .TransformWith(x.Resolve<IEntityDatabaseTransformer>())
                        .SendTo(new FileEndpoint(DefaultEntityDatabaseFile))
                        .Build();
                }));
            
            // These are defaults, you can override these in your own app
            container.Bind<IReceiveDataPipeline>(builder =>
                builder.ToMethod(x => new PipelineBuilder()
                    .RecieveFrom(new FileEndpoint(DefaultEntityDatabaseFile))
                    .DeserializeWith(x.Resolve<IBinaryDeserializer>())
                    .TransformWith(x.Resolve<IEntityDatabaseTransformer>())
                    .Build()));
        }
    }
}