using System;
using EcsRx.Groups;

namespace EcsRx.Plugins.GroupBinding.Attributes
{
    /// <summary>
    /// Will attempt to auto populate an ObservableGroup with a provided required components
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FromComponentsAttribute : Attribute
    {
        public IGroup Group { get; }

        public FromComponentsAttribute(params Type[] requiredComponents)
        { Group = new Group(requiredComponents); }
    }
}