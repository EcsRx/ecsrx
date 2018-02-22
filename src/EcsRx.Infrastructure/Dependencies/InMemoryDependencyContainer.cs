using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Infrastructure.Dependencies
{
    /// <summary>
    /// This is a bare bones implementation, it is recommended you replace this with a Ninject, Zenject, Autofac style implementation
    /// </summary>
    public class InMemoryDependencyContainer : IDependencyContainer
    {
        private readonly IDictionary<Type, IList<BindingConfiguration>> _bindingConfigurations;
        private readonly IDictionary<Type, IList<object>> _dependencies;
        
        public object UnderlyingContainer => _dependencies;

        public InMemoryDependencyContainer()
        {
            _dependencies = new Dictionary<Type, IList<object>>();
            _bindingConfigurations = new Dictionary<Type, IList<BindingConfiguration>>();
        }

        public void Bind<TFrom, TTo>(BindingConfiguration configuration = null) where TTo : TFrom
        {
            throw new System.NotImplementedException();
        }

        public T Resolve<T>(string name = null)
        { return (T)Resolve(typeof(T)); }

        public IEnumerable<T> ResolveAll<T>()
        {
            var type = typeof(T);
            return ResolveAll(type).Cast<T>();
        }
        
        public IEnumerable<object> ResolveAll(Type type)
        {
            if (_dependencies.ContainsKey(type))
            { return _dependencies[type]; }
            
            var bindingConfigs = _bindingConfigurations[type];
            foreach(var bindingConfig in bindingConfigs)
            { ProcessBinding(type, bindingConfig); }

            return _dependencies[type];
        }

        public object Resolve(Type type)
        {
            if (_dependencies.ContainsKey(type))
            { return _dependencies[type].First(); }

            var bindingConfigs = _bindingConfigurations[type];
            foreach(var bindingConfig in bindingConfigs)
            { ProcessBinding(type, bindingConfig); }
            
            return _dependencies[type].First(); 
        }

        public void ProcessBinding(Type type, BindingConfiguration bindingConfig)
        {
            if (bindingConfig.BindInstance != null)
            {
                _dependencies[type] = new List<object> {bindingConfig.BindInstance};
                return;
            }
            
            var instantiatedType = InstantiateType(type);

            if (_dependencies.ContainsKey(type))
            { _dependencies[type].Add(instantiatedType); }
            
            _dependencies[type] = new List<object> { instantiatedType };
        }

        public object InstantiateType(Type type)
        {
            var constructors = type.GetConstructors().Where(x => x.IsPublic);
            var usingConstructor = constructors.First();

            var parameters = usingConstructor.GetParameters();
            var constructorArgs = new object[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            { constructorArgs[i] = Resolve(parameters[i].ParameterType); }

            return usingConstructor.Invoke(constructorArgs);
        }
    }
}