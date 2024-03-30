using EcsRx.Components;
using SystemsRx.Plugins.Transforms.Models;

namespace EcsRx.Plugins.Transforms.Components
{
    public class TransformComponent : IComponent
    {
        /// <summary>
        /// The transform of the component
        /// </summary>
        public Transform Transform { get; set; } = new Transform();
    }
}