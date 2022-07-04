using System;
using System.Collections.Generic;
using System.Linq;

namespace EcsRx.Groups
{
    public class Group : IGroup
    {
		public static readonly IGroup Empty = new EmptyGroup();

		public Type[] RequiredComponents { get; }
	    public Type[] ExcludedComponents { get; }

		/// <summary>
		/// Hint: maybe consider using Group.Empty for better performance, unless you plan on changing the group afterwards.
		/// </summary>
		public Group()
		{
			RequiredComponents = Array.Empty<Type>();
			ExcludedComponents = Array.Empty<Type>();
		}

		public Group(params Type[] requiredComponents)
		{
			RequiredComponents = requiredComponents?.ToArray() ?? Array.Empty<Type>();
			ExcludedComponents = Array.Empty<Type>();
		}
        
	    public Group(IEnumerable<Type> requiredComponents, IEnumerable<Type> excludedComponents)
	    {
            RequiredComponents = requiredComponents?.ToArray() ?? Array.Empty<Type>();
		    ExcludedComponents = excludedComponents?.ToArray() ?? Array.Empty<Type>();
	    }
    }
}