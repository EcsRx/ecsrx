using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Mappings.Types.Primitives;
using LazyData.Mappings.Types.Primitives.Checkers;
using LazyData.Registries;
using LazyData.Serialization.Binary;
using LazyData.Serialization.Binary.Handlers;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class LazyDataModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            container.Bind<TypeAnalyzerConfiguration>(x => x.AsSingleton());

            var primitiveRegistry = new PrimitiveRegistry();
            primitiveRegistry.AddPrimitiveCheck(new BasicPrimitiveChecker());

            container.Bind<IPrimitiveRegistry>(x =>
            {
                x.ToInstance(primitiveRegistry)
                    .AsSingleton();
            });
            
            container.Bind<ITypeCreator, TypeCreator>();
            container.Bind<ITypeAnalyzer, TypeAnalyzer>();
            container.Bind<ITypeMapper, EverythingTypeMapper>();
            container.Bind<IMappingRegistry, MappingRegistry>();
            
            container.Bind<IBinaryPrimitiveHandler, BasicBinaryPrimitiveHandler>();
            container.Bind<IBinarySerializer, BinarySerializer>();
            container.Bind<IBinaryDeserializer, BinaryDeserializer>();
        }
    }
}