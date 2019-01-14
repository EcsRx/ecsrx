using System;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Extensions;
using EcsRx.Groups.Observable;
using EcsRx.MicroRx.Disposables;
using EcsRx.MicroRx.Extensions;
using EcsRx.Plugins.Batching.Builders;
using EcsRx.Plugins.Batching.Descriptors;

namespace EcsRx.Plugins.Batching.Accessors
{
    public abstract class BatchAccessor : IBatchAccessor
    {
        public IObservableGroup ObservableGroup { get; }
        public IComponentPool ComponentPool { get; }
        public IBatchBuilder BatchBuilder { get; }

        protected IDisposable Subscriptions;
        
        protected BatchAccessor(IObservableGroup observableGroup, IComponentPool componentPool, IBatchBuilder batchBuilder)
        {
            ObservableGroup = observableGroup;
            ComponentPool = componentPool;
            BatchBuilder = batchBuilder;
        }

        protected void SetupAccessor()
        {
            var subscriptions = new CompositeDisposable();
            ObservableGroup.OnEntityAdded.Subscribe(_ => Refresh()).AddTo(subscriptions);
            ObservableGroup.OnEntityRemoved.Subscribe(_ => Refresh()).AddTo(subscriptions);
        }

        protected void Refresh()
        {
            
        }
    }
    
    public interface IBatchAccessor
    {
    }

    public interface IBatchAccessor<T1> : IBatchAccessor
        where T1 : unmanaged, IComponent
    {
        Batch<T1>[] Batch { get; }
    }
       
    public interface IBatchAccessor<T1, T2> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        Batch<T1, T2>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        Batch<T1, T2, T3>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4, T5> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5>[] Batch { get; }
    }
    
    public interface IBatchAccessor<T1, T2, T3, T4, T5, T6> : IBatchAccessor
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5, T6>[] Batch { get; }
    }
    
}