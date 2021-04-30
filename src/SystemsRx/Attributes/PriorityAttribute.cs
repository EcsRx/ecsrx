using System;

namespace SystemsRx.Attributes
{
    /// <summary>
    /// Specifies the priority of a system.
    /// </summary>
    /// <remarks>
    /// The ordering will be from highest to lowest so if you have a priority of 10
    /// it will load before a system with a priority of 1. You can also use negative
    /// priorities if you want something to run AFTER the defaults, so -1 would run
    /// after default then -100 would run after that, making the order:
    /// 100, 1, 0, -1, -100
    /// </remarks>
    /// <example>
    /// Shows the running order of common use cases
    /// <code>
    /// [Priority(100)]  public class FirstRunner() {}   // Runs first
    /// [Priority(1)] public class SecondRunner() {}     // Runs second
    /// public class ThirdRunner() {}                    // Runs third
    /// [Priority(-1)] public class FourthRunner() {}    // Runs fourth
    /// [Priority(-100)] public class FifthRunner() {}   // Runs last
    /// </code>
    /// </example>
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