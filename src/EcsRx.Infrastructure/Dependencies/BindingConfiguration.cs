using System.Collections.Generic;

namespace EcsRx.Infrastructure.Dependencies
{
    public class BindingConfiguration
    {
        public bool AsSingleton { get; set; }  
        public string WithName { get; set; }
        public object BindInstance { get; set; }
        public IDictionary<string, object> WithConstructorArgs { get; set; }
    }
}