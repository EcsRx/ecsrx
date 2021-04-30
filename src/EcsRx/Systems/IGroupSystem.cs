using SystemsRx.Systems;
using EcsRx.Groups;

namespace EcsRx.Systems
{
    /// <summary>
    /// The base interface for all systems, this is rarely used directly
    /// </summary>
    public interface IGroupSystem : ISystem
    {
        /// <summary>
        /// The group to target with this system
        /// </summary>
        IGroup Group { get; }
    }
}