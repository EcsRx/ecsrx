using SystemsRx.Extensions;
using SystemsRx.Infrastructure.Dependencies;
using SystemsRx.Infrastructure.Extensions;
using LazyData.Binary;
using LazyData.Binary.Handlers;
using LazyData.Mappings.Mappers;
using LazyData.Mappings.Types;
using LazyData.Mappings.Types.Primitives;
using LazyData.Mappings.Types.Primitives.Checkers;
using LazyData.Registries;

namespace EcsRx.Plugins.Persistence.Modules
{
    public class LazyDataModule : IDependencyModule
    {
        public void Setup(IDependencyRegistry registry)
        {
            registry.Bind<TypeAnalyzerConfiguration>(x => x.AsSingleton());

            registry.Bind<IPrimitiveChecker, BasicPrimitiveChecker>();
            
            registry.Bind<IPrimitiveRegistry>(builder =>
                builder
                    .ToMethod(x =>
                        {
                            var primitiveCheckers = x.ResolveAll<IPrimitiveChecker>();
                            var primitiveRegistry = new PrimitiveRegistry();
                            primitiveCheckers.ForEachRun(primitiveRegistry.AddPrimitiveCheck);
                            return primitiveRegistry;
                        })
                    .AsSingleton());

            registry.Bind<ITypeCreator, TypeCreator>();
            registry.Bind<ITypeAnalyzer, TypeAnalyzer>();
            registry.Bind<MappingConfiguration>(x => x.ToInstance(MappingConfiguration.Default)); 
            registry.Bind<EverythingTypeMapper>();
            registry.Bind<DefaultTypeMapper>();
            registry.Bind<ITypeMapper>(x => x.ToBoundType<DefaultTypeMapper>());
            registry.Bind<IMappingRegistry, MappingRegistry>();

            registry.Bind<IBinaryPrimitiveHandler, BasicBinaryPrimitiveHandler>();
            registry.Bind<IBinarySerializer, BinarySerializer>();
            registry.Bind<IBinaryDeserializer, BinaryDeserializer>();
        }
    }
}