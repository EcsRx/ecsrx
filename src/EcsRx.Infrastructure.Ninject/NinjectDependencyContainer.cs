using System;
using System.Collections;
using System.Linq;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Dependencies;
using Ninject;
using Ninject.Syntax;

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
        
        public void Bind(Type fromType, Type toType, BindingConfiguration configuration = null)
        {
            var bindingSetup = _kernel.Bind(fromType);
            
            if (configuration == null)
            {
                bindingSetup.To(toType).InSingletonScope();
                return;
            }

            IBindingWhenInNamedWithOrOnSyntax<object> binding;

            if (configuration.ToInstance != null)
            { binding = bindingSetup.ToConstant(configuration.ToInstance); }
            else if (configuration.ToMethod != null)
            { binding = bindingSetup.ToMethod(x => configuration.ToMethod(this)); }
            else
            {
                binding = bindingSetup.To(toType);
                
                foreach (var constructorArg in configuration.WithNamedConstructorArgs)
                { binding.WithConstructorArgument(constructorArg.Key, constructorArg.Value); }
            
                foreach (var constructorArg in configuration.WithTypedConstructorArgs)
                { binding.WithConstructorArgument(constructorArg.Key, constructorArg.Value); }
            }
            
            if(configuration.AsSingleton)
            { binding.InSingletonScope(); }
            
            if(!string.IsNullOrEmpty(configuration.WithName))
            { binding.Named(configuration.WithName); }

            if (configuration.OnActivation != null)
            { binding.OnActivation(instance => configuration.OnActivation(this, instance)); }

            if (configuration.WhenInjectedInto.Count != 0)
            { configuration.WhenInjectedInto.ForEachRun(x => binding.WhenInjectedInto(x)); }
        }

        public void Bind(Type type, BindingConfiguration configuration = null)
        { Bind(type, type, configuration); }

        public bool HasBinding(Type type, string name = null)
        {
            var applicableBindings = _kernel.GetBindings(type);
             
            if(string.IsNullOrEmpty(name))
            { return applicableBindings.Any(); }
            
            return applicableBindings.Any(x => x.Metadata.Name == name);
        }

        public object Resolve(Type type, string name = null)
        {
            if(string.IsNullOrEmpty(name))
            { return _kernel.Get(type); }

            return _kernel.Get(type, name);
        }

        public void Unbind(Type type)
        { _kernel.Unbind(type); }

        public IEnumerable ResolveAll(Type type)
        { return _kernel.GetAll(type); }

        public void LoadModule(IDependencyModule module)
        { module.Setup(this); }

        public void Dispose()
        { _kernel?.Dispose(); }
    }
}