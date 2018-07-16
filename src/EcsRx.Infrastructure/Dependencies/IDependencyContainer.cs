using System.Collections.Generic;

namespace EcsRx.Infrastructure.Dependencies
{
    /// <summary>
    /// This represents a cross platform way of managing dependencies.
    /// 
    /// It is up to the consumer to implement the actual underlying handler
    /// for unity it will be done out of the box with Zenject, but in other
    /// platforms like Monogame, Godot etc it would be up to the consumer
    /// to define what DI system they want to use and create an implementation
    /// themselves.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// This exposes the underlying DI container, but any calls to this directly
        /// will not be cross platform, so be weary if you need it or not.
        /// </summary>
        object NativeContainer { get; }
        
        void Bind<TFrom, TTo>(BindingConfiguration configuration = null) where TTo : TFrom;
        void Bind<T>(BindingConfiguration configuration = null);

        T Resolve<T>(string name = null);
        IEnumerable<T> ResolveAll<T>();

        void LoadModule<T>() where T : IDependencyModule, new();
    }
}