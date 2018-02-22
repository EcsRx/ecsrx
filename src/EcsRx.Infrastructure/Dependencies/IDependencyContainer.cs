using System.Collections.Generic;

namespace EcsRx.Infrastructure.Dependencies
{
    public interface IDependencyContainer
    {
        object UnderlyingContainer { get; }
        
        void Bind<TFrom, TTo>(BindingConfiguration configuration = null) where TTo : TFrom;
        T Resolve<T>(string name = null);
        IEnumerable<T> ResolveAll<T>();
    }
}