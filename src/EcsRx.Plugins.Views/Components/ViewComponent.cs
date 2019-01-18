using EcsRx.Components;

namespace EcsRx.Plugins.Views.Components
{
    public class ViewComponent : IComponent
    {
        public bool DestroyWithView { get; set; }
        public object View { get; set; }
    }
}