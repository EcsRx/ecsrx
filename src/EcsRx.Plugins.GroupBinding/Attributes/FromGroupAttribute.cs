using System;
using EcsRx.Groups;

namespace EcsRx.Plugins.GroupBinding.Attributes
{
    /// <summary>
    /// Will attempt to auto populate an ObservableGroup with a provided group or IGroupSystem Group property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FromGroupAttribute : Attribute
    {
        public IGroup Group { get; }

        public FromGroupAttribute(Type group = null)
        { Group = group == null ? null : (IGroup)Activator.CreateInstance(group); }
    }
}