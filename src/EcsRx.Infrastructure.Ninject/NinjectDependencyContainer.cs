using System.Collections.Generic;
using EcsRx.Infrastructure.Dependencies;
using Ninject;

namespace EcsRx.Infrastructure.Ninject
{
    /// <summary>
    /// This is a ninject implementation for the dependency container.
    /// 
    /// As with all the dependency container implementations, you should implement
    /// a basic way to bind/resolve/resolveall but as some DI config may be more
    /// complex the underlying container should be exposed to the consumer so they
    /// can make use of native features if needed.
    /// 
    /// One thing to mention though is if any native container calls are used
    /// they will only be compatible with that dependency container, so when making
    /// plugins you ideally want to stick to the methods exposed on the interface
    /// to make your stuff cross platform.
    /// </summary>
    public class NinjectDependencyContainer : IDependencyContainer
    {
        private readonly IKernel _kernel;

        public NinjectDependencyContainer(IKernel kernel = null)
        {
            _kernel = kernel ?? new StandardKernel();
        }

        public object NativeContainer => _kernel;
        
        public void Bind<TFrom, TTo>(BindingConfiguration configuration = null) where TTo : TFrom
        {
            var bindingSetup = _kernel.Bind<TFrom>();
            
            if (configuration == null)
            {
                bindingSetup.To<TTo>().InSingletonScope();
                return;
            }

            if (configuration.ToInstance != null)
            {
                var instanceBinding = bindingSetup.ToConstant((TFrom) configuration.ToInstance);
                
                if(configuration.AsSingleton)
                { instanceBinding.InSingletonScope(); }

                return;
            }

            if (configuration.ToMethod != null)
            {
                var methodBinding = bindingSetup.ToMethod(x => (TTo)configuration.ToMethod(this));

                if(configuration.AsSingleton)
                { methodBinding.InSingletonScope(); }

                return;
            }

            var binding = bindingSetup.To<TFrom>();
            
            if(configuration.AsSingleton)
            { binding.InSingletonScope(); } 
            
            if(!string.IsNullOrEmpty(configuration.WithName))
            { binding.Named(configuration.WithName); }

            if (configuration.WithNamedConstructorArgs.Count == 0)
            { return; }

            foreach (var constructorArg in configuration.WithNamedConstructorArgs)
            { binding.WithConstructorArgument(constructorArg.Key, constructorArg.Value); }
        }

        public void Bind<T>(BindingConfiguration configuration = null)
        { Bind<T,T>(configuration); }

        public T Resolve<T>(string name = null)
        {
            if(string.IsNullOrEmpty(name))
            { return _kernel.Get<T>(); }

            return _kernel.Get<T>(name);
        }

        public void Unbind<T>()
        {
            _kernel.Unbind<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        { return _kernel.GetAll<T>(); }

        public void LoadModule<T>() where T : IDependencyModule, new()
        {
            var module = new T();
            LoadModule(module);
        }
        
        public void LoadModule(IDependencyModule module)
        { module.Setup(this); }
    }
}