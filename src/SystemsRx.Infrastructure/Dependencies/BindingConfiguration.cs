using System;
using System.Collections.Generic;

namespace SystemsRx.Infrastructure.Dependencies
{
    public class BindingConfiguration
    {

        public bool AsSingleton { get; set; }  
        public string WithName { get; set; }
        public object ToInstance { get; set; }
        public IList<Type> WhenInjectedInto { get; set; }
        public Func<IDependencyContainer, object> ToMethod { get; set; }
        public Action<IDependencyContainer, object> OnActivation { get; set; }
        public IDictionary<string, object> WithNamedConstructorArgs { get; }
        public IDictionary<Type, object> WithTypedConstructorArgs { get; }

        public BindingConfiguration()
        {
            AsSingleton = true;       
            WithNamedConstructorArgs = new Dictionary<string, object>();
            WithTypedConstructorArgs = new Dictionary<Type, object>();
            WhenInjectedInto = new List<Type>();
        }
    }
}