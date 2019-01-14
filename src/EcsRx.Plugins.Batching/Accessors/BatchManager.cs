using System.Collections.Generic;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Plugins.Batching.Accessors
{   
    public class BatchManager : IBatchManager
    {
        public Dictionary<int[], IBatchAccessor> BatchAccessors { get; } = new Dictionary<int[], IBatchAccessor>();
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IBatchBuilderFactory BatchBuilderFactory { get; }
        public IReferenceBatchBuilderFactory ReferenceBatchBuilderFactory { get; }
        public IObservableGroupManager ObservableGroupManager { get; }

        public BatchManager(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IObservableGroupManager observableGroupManager)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            BatchBuilderFactory = batchBuilderFactory;
            ObservableGroupManager = observableGroupManager;
        }

        public IBatchAccessor GetAccessorFor<T1>() 
            where T1 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1));
            if (BatchAccessors.ContainsKey(componentTypes))
            { return (IBatchAccessor<T1>) BatchAccessors[componentTypes]; }

            var batchBuilder = BatchBuilderFactory.Create<T1>();
            var initialBatch = batchBuilder.Build()
        }

        public IBatchAccessor GetAccessorFor<T1, T2>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent
        {
            throw new System.NotImplementedException();
        }

        public IBatchAccessor GetAccessorFor<T1, T2, T3>() 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent
        {
            throw new System.NotImplementedException();
        }

        public IBatchAccessor GetAccessorFor<T1, T2, T3, T4>() where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent
        {
            throw new System.NotImplementedException();
        }

        public IBatchAccessor GetAccessorFor<T1, T2, T3, T4, T5>() where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent
        {
            throw new System.NotImplementedException();
        }

        public IBatchAccessor GetAccessorFor<T1, T2, T3, T4, T5, T6>() where T1 : unmanaged, IComponent where T2 : unmanaged, IComponent where T3 : unmanaged, IComponent where T4 : unmanaged, IComponent where T5 : unmanaged, IComponent where T6 : unmanaged, IComponent
        {
            throw new System.NotImplementedException();
        }
    }
}