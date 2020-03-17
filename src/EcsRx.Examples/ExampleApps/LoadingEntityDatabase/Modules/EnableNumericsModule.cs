using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using LazyData.Binary.Handlers;
using LazyData.Json.Handlers;
using LazyData.Mappings.Types.Primitives;
using LazyData.Numerics.Checkers;
using LazyData.Numerics.Handlers;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class EnableNumericsModule
    {
        public void Setup(IDependencyContainer container)
        {
            // For this one lets tell LazyData how to handle numeric types
            // For more info on this stuff look at the LazyData project docs
            var primitiveRegistry = container.Resolve<IPrimitiveRegistry>();
            primitiveRegistry.AddPrimitiveCheck(new NumericsPrimitiveChecker());
            
            // Tell it how to handle objects in System.Numerics in json world
            container.Bind<IJsonPrimitiveHandler, NumericsJsonPrimitiveHandler>();
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();

            
            // Tell it how to handle objects in System.Numerics in binary world
            container.Bind<IBinaryPrimitiveHandler, NumericsBinaryPrimitiveHandler>();
        }
    }
}