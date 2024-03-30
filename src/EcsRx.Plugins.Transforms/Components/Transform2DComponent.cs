using EcsRx.Components;
using SystemsRx.Plugins.Transforms.Models;

namespace EcsRx.Plugins.Transforms.Components
{
    public class Transform2DComponent : IComponent
    {
        /// <summary>
        /// The transform of the component
        /// </summary>
        public Transform2D Transform { get; set; } = new Transform2D();
    }
}