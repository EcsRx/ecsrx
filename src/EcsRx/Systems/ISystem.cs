using EcsRx.Groups;

namespace EcsRx.Systems
{
    /// <summary>
    /// The base interface for all systems, this is rarely used directly
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// The group to target with this system
        /// </summary>
        IGroup Group { get; }
    }
}