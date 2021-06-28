using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Builders
{   
    public unsafe class BatchBuilder<T1, T2> : IBatchBuilder<T1, T2> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2;

        public BatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
        }

        public PinnedBatch<T1, T2> Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            
            var batches = new Batch<T1, T2>[entities.Count];
            
            var component1Handle  = GCHandle.Alloc(componentArray1, GCHandleType.Pinned);
            var component2Handle  = GCHandle.Alloc(componentArray2, GCHandleType.Pinned);
            var component1Pointer = (T1*)component1Handle.AddrOfPinnedObject().ToPointer();
            var component2Pointer = (T2*)component2Handle.AddrOfPinnedObject().ToPointer();

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                {
                    var tempBatch = Array.Empty<Batch<T1, T2>>();
                    return new PinnedBatch<T1, T2>(tempBatch, Array.Empty<GCHandle>());
                }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                batches[i] = new Batch<T1, T2>(entity.Id, &component1Pointer[component1Allocation], 
                    &component2Pointer[component2Allocation]);
            }
            
            return new PinnedBatch<T1, T2>(batches, new[] { component1Handle, component2Handle});
        }
    }
    
    public unsafe class BatchBuilder<T1, T2, T3> : IBatchBuilder<T1, T2, T3> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3;

        public BatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
        }

        public PinnedBatch<T1, T2, T3> Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            
            var batches = new Batch<T1, T2, T3>[entities.Count];
            var component1Handle  = GCHandle.Alloc(componentArray1, GCHandleType.Pinned);
            var component2Handle  = GCHandle.Alloc(componentArray2, GCHandleType.Pinned);
            var component3Handle  = GCHandle.Alloc(componentArray3, GCHandleType.Pinned);
            var component1Pointer = (T1*)component1Handle.AddrOfPinnedObject().ToPointer();
            var component2Pointer = (T2*)component2Handle.AddrOfPinnedObject().ToPointer();
            var component3Pointer = (T3*)component3Handle.AddrOfPinnedObject().ToPointer();
            
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                {
                    var tempBatch = Array.Empty<Batch<T1, T2, T3>>();
                    return new PinnedBatch<T1, T2, T3>(tempBatch, Array.Empty<GCHandle>());
                }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                batches[i] = new Batch<T1, T2, T3>(
                    entity.Id, &component1Pointer[component1Allocation], 
                    &component2Pointer[component2Allocation],
                    &component3Pointer[component3Allocation]);
            }
            
            return new PinnedBatch<T1, T2, T3>(batches, new []{ component1Handle, component2Handle, component3Handle});
        }
    }
    
    public unsafe class BatchBuilder<T1, T2, T3, T4> : IBatchBuilder<T1, T2, T3, T4> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, _componentTypeId4;

        public BatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
        }

        public PinnedBatch<T1, T2, T3, T4> Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            
            var batches = new Batch<T1, T2, T3, T4>[entities.Count];

            var component1Handle  = GCHandle.Alloc(componentArray1, GCHandleType.Pinned);
            var component2Handle  = GCHandle.Alloc(componentArray2, GCHandleType.Pinned);
            var component3Handle  = GCHandle.Alloc(componentArray3, GCHandleType.Pinned);
            var component4Handle  = GCHandle.Alloc(componentArray4, GCHandleType.Pinned);
            var component1Pointer = (T1*)component1Handle.AddrOfPinnedObject().ToPointer();
            var component2Pointer = (T2*)component2Handle.AddrOfPinnedObject().ToPointer();
            var component3Pointer = (T3*)component3Handle.AddrOfPinnedObject().ToPointer();
            var component4Pointer = (T4*)component4Handle.AddrOfPinnedObject().ToPointer();
            
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                {
                    var tempBatch = Array.Empty<Batch<T1, T2, T3, T4>>();
                    return new PinnedBatch<T1, T2, T3, T4>(tempBatch, Array.Empty<GCHandle>());
                }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                batches[i] = new Batch<T1, T2, T3, T4>(
                    entity.Id, &component1Pointer[component1Allocation], &component2Pointer[component2Allocation],
                    &component3Pointer[component3Allocation], &component4Pointer[component4Allocation]);
            }
            
            return new PinnedBatch<T1, T2, T3, T4>(batches,
                new[] {component1Handle, component2Handle, component3Handle, component4Handle});
        }
    }
    
    public unsafe class BatchBuilder<T1, T2, T3, T4, T5> : IBatchBuilder<T1, T2, T3, T4, T5> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, 
            _componentTypeId4, _componentTypeId5;

        public BatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
            _componentTypeId5 = componentTypeLookup.GetComponentType(typeof(T5));
        }

        public PinnedBatch<T1, T2, T3, T4, T5> Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            
            var batches = new Batch<T1, T2, T3, T4, T5>[entities.Count];

            var component1Handle  = GCHandle.Alloc(componentArray1, GCHandleType.Pinned);
            var component2Handle  = GCHandle.Alloc(componentArray2, GCHandleType.Pinned);
            var component3Handle  = GCHandle.Alloc(componentArray3, GCHandleType.Pinned);
            var component4Handle  = GCHandle.Alloc(componentArray4, GCHandleType.Pinned);
            var component5Handle  = GCHandle.Alloc(componentArray5, GCHandleType.Pinned);
            var component1Pointer = (T1*)component1Handle.AddrOfPinnedObject().ToPointer();
            var component2Pointer = (T2*)component2Handle.AddrOfPinnedObject().ToPointer();
            var component3Pointer = (T3*)component3Handle.AddrOfPinnedObject().ToPointer();
            var component4Pointer = (T4*)component4Handle.AddrOfPinnedObject().ToPointer();
            var component5Pointer = (T5*)component5Handle.AddrOfPinnedObject().ToPointer();
            
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                {
                    var tempBatch = Array.Empty<Batch<T1, T2, T3, T4, T5>>();
                    return new PinnedBatch<T1, T2, T3, T4, T5>(tempBatch, Array.Empty<GCHandle>());
                }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                batches[i] = new Batch<T1, T2, T3, T4, T5>(
                    entity.Id, &component1Pointer[component1Allocation], &component2Pointer[component2Allocation],
                    &component3Pointer[component3Allocation], &component4Pointer[component4Allocation],
                    &component5Pointer[component5Allocation]);
            }

            return new PinnedBatch<T1, T2, T3, T4, T5>(batches,
                new[] { component1Handle, component2Handle, component3Handle, component4Handle, component5Handle});
        }
    }
    
    public unsafe class BatchBuilder<T1, T2, T3, T4, T5, T6> : IBatchBuilder<T1, T2, T3, T4, T5, T6> 
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, 
            _componentTypeId4, _componentTypeId5, _componentTypeId6;

        public BatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
            _componentTypeId5 = componentTypeLookup.GetComponentType(typeof(T5));
            _componentTypeId6 = componentTypeLookup.GetComponentType(typeof(T6));
        }

        public PinnedBatch<T1, T2, T3, T4, T5, T6> Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            var componentArray6 = ComponentDatabase.GetComponents<T6>(_componentTypeId6);
            
            var batches = new Batch<T1, T2, T3, T4, T5, T6>[entities.Count];

            var component1Handle  = GCHandle.Alloc(componentArray1, GCHandleType.Pinned);
            var component2Handle  = GCHandle.Alloc(componentArray2, GCHandleType.Pinned);
            var component3Handle  = GCHandle.Alloc(componentArray3, GCHandleType.Pinned);
            var component4Handle  = GCHandle.Alloc(componentArray4, GCHandleType.Pinned);
            var component5Handle  = GCHandle.Alloc(componentArray5, GCHandleType.Pinned);
            var component6Handle  = GCHandle.Alloc(componentArray6, GCHandleType.Pinned);
            var component1Pointer = (T1*)component1Handle.AddrOfPinnedObject().ToPointer();
            var component2Pointer = (T2*)component2Handle.AddrOfPinnedObject().ToPointer();
            var component3Pointer = (T3*)component3Handle.AddrOfPinnedObject().ToPointer();
            var component4Pointer = (T4*)component4Handle.AddrOfPinnedObject().ToPointer();
            var component5Pointer = (T5*)component5Handle.AddrOfPinnedObject().ToPointer();
            var component6Pointer = (T6*)component6Handle.AddrOfPinnedObject().ToPointer();
            
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                {
                    var tempBatch = Array.Empty<Batch<T1, T2, T3, T4, T5, T6>>();
                    return new PinnedBatch<T1, T2, T3, T4, T5, T6>(tempBatch, Array.Empty<GCHandle>());
                }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                var component6Allocation = entity.ComponentAllocations[_componentTypeId6];
                batches[i] = new Batch<T1, T2, T3, T4, T5, T6>(
                    entity.Id, &component1Pointer[component1Allocation], &component2Pointer[component2Allocation],
                    &component3Pointer[component3Allocation], &component4Pointer[component4Allocation],
                    &component5Pointer[component5Allocation], &component6Pointer[component6Allocation]);
            }

            return new PinnedBatch<T1, T2, T3, T4, T5, T6>(batches, 
                new[] { component1Handle, component2Handle, component3Handle, 
                component4Handle, component5Handle, component6Handle});
        }
    }
}