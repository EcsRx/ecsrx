using System;
using EcsRx.Infrastructure.Exceptions;

namespace EcsRx.Infrastructure.Dependencies
{
    public class BindingBuilder
    {
        protected readonly BindingConfiguration _configuration = new BindingConfiguration();

        public BindingBuilder AsSingleton()
        {
            _configuration.AsSingleton = true;
            return this;
        }

        public BindingBuilder AsTransient()
        {
            _configuration.AsSingleton = false;
            return this;
        }

        public BindingBuilder WithName(string name)
        {
            _configuration.WithName = name;
            return this;
        }       
        
        public BindingBuilder WithConstructorArg(string argName, object argValue)
        {
            _configuration.WithNamedConstructorArgs.Add(argName, argValue);
            return this;
        }
        
        public BindingBuilder WithConstructorArg<T>(T argValue)
        {
            _configuration.WithTypedConstructorArgs.Add(typeof(T), argValue);
            return this;
        }
        
        public BindingBuilder WithConstructorArg(Type argType, object argValue)
        {
            _configuration.WithTypedConstructorArgs.Add(argType, argValue);
            return this;
        }
        
        internal BindingConfiguration Build()
        {
            return _configuration;
        }
    }
    
    public class BindingBuilder<TFrom> : BindingBuilder
    {
        public BindingBuilder<TFrom> ToInstance<TTo>(TTo instance) where TTo : TFrom
        {
            if(_configuration.ToMethod != null)
            { throw new BindingException("Cannot use instance when a method has been provided already"); }
            
            _configuration.ToInstance = instance;
            return this;
        }
        
        public BindingBuilder<TFrom> ToMethod<TTo>(Func<IDependencyContainer, TTo> method) where TTo : TFrom
        {
            if(_configuration.ToInstance != null)
            { throw new BindingException("Cannot use method when an instance has been provided already"); }
            
            _configuration.ToMethod = container => method(container);
            return this;
        }
    }
}