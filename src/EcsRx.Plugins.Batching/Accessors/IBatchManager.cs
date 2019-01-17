using EcsRx.Components;
using EcsRx.Groups.Observable;

namespace EcsRx.Plugins.Batching.Accessors
{
    public interface IBatchManager
    {
        IBatchAccessor<T1,T2> GetAccessorFor<T1, T2>(IObservableGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2> GetReferenceAccessorFor<T1, T2>(IObservableGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3> GetAccessorFor<T1, T2, T3>(IObservableGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3> GetReferenceAccessorFor<T1, T2, T3>(IObservableGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4> GetAccessorFor<T1, T2, T3, T4>(IObservableGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3,T4> GetReferenceAccessorFor<T1, T2, T3, T4>(IObservableGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4,T5> GetAccessorFor<T1, T2, T3, T4, T5>(IObservableGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent;
        
        IReferenceBatchAccessor<T1,T2,T3,T4,T5> GetReferenceAccessorFor<T1, T2, T3, T4, T5>(IObservableGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent;
        
        IBatchAccessor<T1,T2,T3,T4,T5,T6> GetAccessorFor<T1, T2, T3, T4, T5, T6>(IObservableGroup observableGroup)
            where T1 : unmanaged, IComponent
            where T2 : unmanaged, IComponent
            where T3 : unmanaged, IComponent
            where T4 : unmanaged, IComponent
            where T5 : unmanaged, IComponent
            where T6 : unmanaged, IComponent;

        IReferenceBatchAccessor<T1,T2,T3,T4,T5,T6> GetReferenceAccessorFor<T1, T2, T3, T4, T5, T6>(IObservableGroup observableGroup)
            where T1 : class, IComponent
            where T2 : class, IComponent
            where T3 : class, IComponent
            where T4 : class, IComponent
            where T5 : class, IComponent
            where T6 : class, IComponent;


    }
}