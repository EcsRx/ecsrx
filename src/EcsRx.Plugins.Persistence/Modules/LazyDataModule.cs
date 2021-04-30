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
        public void Setup(IDependencyContainer container)
        {
            container.Bind<TypeAnalyzerConfiguration>(x => x.AsSingleton());

            container.Bind<IPrimitiveChecker, BasicPrimitiveChecker>();
            
            container.Bind<IPrimitiveRegistry>(builder =>
                builder
                    .ToMethod(x =>
                        {
                            var primitiveCheckers = x.ResolveAll<IPrimitiveChecker>();
                            var primitiveRegistry = new PrimitiveRegistry();
                            primitiveCheckers.ForEachRun(primitiveRegistry.AddPrimitiveCheck);
                            return primitiveRegistry;
                        })
                    .AsSingleton());

            container.Bind<ITypeCreator, TypeCreator>();
            container.Bind<ITypeAnalyzer, TypeAnalyzer>();
            container.Bind<MappingConfiguration>(x => x.ToInstance(MappingConfiguration.Default)); 
            container.Bind<EverythingTypeMapper>();
            container.Bind<DefaultTypeMapper>();
            container.Bind<ITypeMapper>(x => x.ToBoundType<DefaultTypeMapper>());
            container.Bind<IMappingRegistry, MappingRegistry>();

            container.Bind<IBinaryPrimitiveHandler, BasicBinaryPrimitiveHandler>();
            container.Bind<IBinarySerializer, BinarySerializer>();
            container.Bind<IBinaryDeserializer, BinaryDeserializer>();
        }
    }
}