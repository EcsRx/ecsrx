using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Batching.Factories;

namespace EcsRx.Plugins.Batching.Accessors
{
    public class BatchManager : IBatchManager
    {
        public Dictionary<AccessorToken, IBatchAccessor> BatchAccessors { get; } = new Dictionary<AccessorToken, IBatchAccessor>();
        public IComponentDatabase ComponentDatabase { get; }
        public IComponentTypeLookup ComponentTypeLookup { get; }
        public IBatchBuilderFactory BatchBuilderFactory { get; }
        public IReferenceBatchBuilderFactory ReferenceBatchBuilderFactory { get; }
        public IObservableGroupManager ObservableGroupManager { get; }

        public BatchManager(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup, IBatchBuilderFactory batchBuilderFactory, IReferenceBatchBuilderFactory referenceBatchBuilderFactory, IObservableGroupManager observableGroupManager)
        {
            ComponentDatabase = componentDatabase;
            ComponentTypeLookup = componentTypeLookup;
            BatchBuilderFactory = batchBuilderFactory;
            ObservableGroupManager = observableGroupManager;
            ReferenceBatchBuilderFactory = referenceBatchBuilderFactory;
        }

        public IBatchAccessor<T1,T2> GetAccessorFor<T1, T2>(IObservableGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IBatchAccessor<T1, T2>) BatchAccessors[token]; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2>();
            var batchAccessor = new BatchAccessor<T1, T2>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2> GetReferenceAccessorFor<T1, T2>(IObservableGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IReferenceBatchAccessor<T1, T2>) BatchAccessors[token]; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1,T2,T3> GetAccessorFor<T1, T2, T3>(IObservableGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IBatchAccessor<T1, T2, T3>) BatchAccessors[token]; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3>();
            var batchAccessor = new BatchAccessor<T1, T2, T3>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2,T3> GetReferenceAccessorFor<T1, T2, T3>(IObservableGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IReferenceBatchAccessor<T1, T2, T3>) BatchAccessors[token]; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1,T2,T3,T4> GetAccessorFor<T1, T2, T3, T4>(IObservableGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IBatchAccessor<T1, T2, T3, T4>) BatchAccessors[token]; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1,T2,T3,T4> GetReferenceAccessorFor<T1, T2, T3, T4>(IObservableGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4>) BatchAccessors[token]; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1, T2, T3, T4, T5> GetAccessorFor<T1, T2, T3, T4, T5>(IObservableGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IBatchAccessor<T1, T2, T3, T4, T5>) BatchAccessors[token]; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4, T5>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4, T5>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1, T2, T3, T4, T5> GetReferenceAccessorFor<T1, T2, T3, T4, T5>(IObservableGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4, T5>) BatchAccessors[token]; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4, T5>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4, T5>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IBatchAccessor<T1, T2, T3, T4, T5,T6> GetAccessorFor<T1, T2, T3, T4, T5, T6>(IObservableGroup observableGroup) 
            where T1 : unmanaged, IComponent 
            where T2 : unmanaged, IComponent 
            where T3 : unmanaged, IComponent 
            where T4 : unmanaged, IComponent 
            where T5 : unmanaged, IComponent 
            where T6 : unmanaged, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IBatchAccessor<T1, T2,T3,T4,T5,T6>) BatchAccessors[token]; }

            var batchBuilder = BatchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
            var batchAccessor = new BatchAccessor<T1, T2, T3, T4, T5, T6>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }

        public IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6> GetReferenceAccessorFor<T1, T2, T3, T4, T5, T6>(IObservableGroup observableGroup) 
            where T1 : class, IComponent 
            where T2 : class, IComponent 
            where T3 : class, IComponent 
            where T4 : class, IComponent 
            where T5 : class, IComponent 
            where T6 : class, IComponent
        {
            var componentTypes = ComponentTypeLookup.GetComponentTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
            var token = new AccessorToken(componentTypes, observableGroup);
            
            if (BatchAccessors.ContainsKey(token))
            { return (IReferenceBatchAccessor<T1, T2, T3, T4, T5, T6>) BatchAccessors[token]; }

            var batchBuilder = ReferenceBatchBuilderFactory.Create<T1, T2, T3, T4, T5, T6>();
            var batchAccessor = new ReferenceBatchAccessor<T1, T2, T3, T4, T5, T6>(observableGroup, ComponentDatabase, batchBuilder, ComponentTypeLookup);
            BatchAccessors.Add(token, batchAccessor);

            return batchAccessor;
        }
    }
}