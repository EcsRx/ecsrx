using EcsRx.Components;

namespace EcsRx.Plugins.Batching.Accessors
{
    public interface IBatchManager
    {
        IBatchAccessor GetAccessorFor<T1>()
            where T1 : unmanaged, IComponent;
        
        IBatchAccessor GetReferenceAccessorFor<T1>()
            where T1 : class, IComponent;
        
        IBatchAccessor GetAccessorFor<T1, T2>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        
        IBatchAccessor GetAccessorFor<T1, T2, T3>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        
        IBatchAccessor GetAccessorFor<T1, T2, T3, T4>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        
        IBatchAccessor GetAccessorFor<T1, T2, T3, T4, T5>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        
        IBatchAccessor GetAccessorFor<T1, T2, T3, T4, T5, T6>()
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;
        
        
    }
}