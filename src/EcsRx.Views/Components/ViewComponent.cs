using EcsRx.Components;

namespace EcsRx.Views.Components
{
    public class ViewComponent : IComponent
    {
        public bool DestroyWithView { get; set; }
        public object View { get; set; }
    }
}