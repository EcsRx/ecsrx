using SystemsRx.Infrastucture.Dependencies;
using SystemsRx.Infrastucture.Extensions;
using LazyData.Binary.Handlers;
using LazyData.Json.Handlers;
using LazyData.Mappings.Types.Primitives.Checkers;
using LazyData.Numerics.Checkers;
using LazyData.Numerics.Handlers;

namespace EcsRx.Examples.ExampleApps.LoadingEntityDatabase.Modules
{
    public class EnableNumericsModule : IDependencyModule
    {
        public void Setup(IDependencyContainer container)
        {
            // For this one lets tell LazyData how to handle numeric types
            // For more info on this stuff look at the LazyData project docs
            container.Bind<IPrimitiveChecker, NumericsPrimitiveChecker>();
            
            // Tell it how to handle objects in System.Numerics in json world
            container.Bind<IJsonPrimitiveHandler, NumericsJsonPrimitiveHandler>();
            container.Bind<IJsonPrimitiveHandler, BasicJsonPrimitiveHandler>();
            
            // Dont need this, but just showing how you load the numerics handler for binary formats
            container.Bind<IBinaryPrimitiveHandler, NumericsBinaryPrimitiveHandler>();
        }
    }
}