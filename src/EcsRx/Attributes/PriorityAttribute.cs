using System;

namespace EcsRx.Attributes
{
    /// <summary>
    /// Specifies the priority of a system.
    /// </summary>
    /// <remarks>
    /// The ordering will be from lowest to highest so if you have a priority of 1
    /// it will load before a system with a priority of 10. You can also use negative
    /// priorities if you REALLY need to push something in front of everything else
    /// but be weary of doing so.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class PriorityAttribute : Attribute
    {
        public int Priority { get; }

        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}