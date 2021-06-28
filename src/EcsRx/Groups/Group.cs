using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Groups
{
    public class Group : IGroup
    {
        public Type[] RequiredComponents { get; }
	    public Type[] ExcludedComponents { get; }
        
		public Group(params Type[] requiredComponents) : this(requiredComponents, Array.Empty<Type>()) {}
        
	    public Group(IEnumerable<Type> requiredComponents, IEnumerable<Type> excludedComponents)
	    {
		    RequiredComponents = requiredComponents.ToArray();
		    ExcludedComponents = excludedComponents.ToArray();
	    }
    }
}