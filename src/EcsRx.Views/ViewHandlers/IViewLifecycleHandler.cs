namespace EcsRx.Views.ViewHandlers
{
    public interface IViewLifecycleHandler
    {
        void DestroyView(object view);
        void SetupView();
    }
}