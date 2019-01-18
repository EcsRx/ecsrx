namespace EcsRx.Plugins.Views.Pooling
{
    public class ViewObjectContainer
    {
        public object ViewObject { get; }
        public bool IsInUse { get; set; }

        public ViewObjectContainer(object viewObject)
        {
            ViewObject = viewObject;
        }
    }
}