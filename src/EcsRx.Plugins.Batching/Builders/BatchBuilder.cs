using System.Collections.Generic;
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

        public Batch<T1, T2>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            
            var batches = new Batch<T1, T2>[entities.Count];

            fixed (T1* component1Handle = componentArray1)
            fixed (T2* component2Handle = componentArray2)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities.Count != batches.Length)
                    { return new Batch<T1, T2>[0]; }
                    
                    var entity = entities[i];
                    var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                    var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                    batches[i] = new Batch<T1, T2>(entity.Id, &component1Handle[component1Allocation], &component2Handle[component2Allocation]);
                }
            }

            return batches;
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

        public Batch<T1, T2, T3>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            
            var batches = new Batch<T1, T2, T3>[entities.Count];

            fixed (T1* component1Handle = componentArray1)
            fixed (T2* component2Handle = componentArray2)
            fixed (T3* component3Handle = componentArray3)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities.Count != batches.Length)
                    { return new Batch<T1, T2, T3>[0]; }
                    
                    var entity = entities[i];
                    var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                    var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                    var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                    batches[i] = new Batch<T1, T2, T3>(
                        entity.Id, &component1Handle[component1Allocation], &component2Handle[component2Allocation],
                        &component3Handle[component3Allocation]);
                }
            }

            return batches;
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

        public Batch<T1, T2, T3, T4>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            
            var batches = new Batch<T1, T2, T3, T4>[entities.Count];

            fixed (T1* component1Handle = componentArray1)
            fixed (T2* component2Handle = componentArray2)
            fixed (T3* component3Handle = componentArray3)
            fixed (T4* component4Handle = componentArray4)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities.Count != batches.Length)
                    { return new Batch<T1, T2, T3, T4>[0]; }
                    
                    var entity = entities[i];
                    var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                    var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                    var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                    var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                    batches[i] = new Batch<T1, T2, T3, T4>(
                        entity.Id, &component1Handle[component1Allocation], &component2Handle[component2Allocation],
                        &component3Handle[component3Allocation], &component4Handle[component4Allocation]);
                }
            }

            return batches;
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

        public Batch<T1, T2, T3, T4, T5>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            
            var batches = new Batch<T1, T2, T3, T4, T5>[entities.Count];

            fixed (T1* component1Handle = componentArray1)
            fixed (T2* component2Handle = componentArray2)
            fixed (T3* component3Handle = componentArray3)
            fixed (T4* component4Handle = componentArray4)
            fixed (T5* component5Handle = componentArray5)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities.Count != batches.Length)
                    { return new Batch<T1, T2, T3, T4, T5>[0]; }
                    
                    var entity = entities[i];
                    var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                    var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                    var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                    var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                    var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                    batches[i] = new Batch<T1, T2, T3, T4, T5>(
                        entity.Id, &component1Handle[component1Allocation], &component2Handle[component2Allocation],
                        &component3Handle[component3Allocation], &component4Handle[component4Allocation],
                        &component5Handle[component5Allocation]);
                }
            }

            return batches;
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

        public Batch<T1, T2, T3, T4, T5, T6>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            var componentArray6 = ComponentDatabase.GetComponents<T6>(_componentTypeId6);
            
            var batches = new Batch<T1, T2, T3, T4, T5, T6>[entities.Count];

            fixed (T1* component1Handle = componentArray1)
            fixed (T2* component2Handle = componentArray2)
            fixed (T3* component3Handle = componentArray3)
            fixed (T4* component4Handle = componentArray4)
            fixed (T5* component5Handle = componentArray5)
            fixed (T6* component6Handle = componentArray6)
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities.Count != batches.Length)
                    { return new Batch<T1, T2, T3, T4, T5, T6>[0]; }
                    
                    var entity = entities[i];
                    var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                    var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                    var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                    var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                    var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                    var component6Allocation = entity.ComponentAllocations[_componentTypeId6];
                    batches[i] = new Batch<T1, T2, T3, T4, T5, T6>(
                        entity.Id, &component1Handle[component1Allocation], &component2Handle[component2Allocation],
                        &component3Handle[component3Allocation], &component4Handle[component4Allocation],
                        &component5Handle[component5Allocation], &component6Handle[component6Allocation]);
                }
            }

            return batches;
        }
    }
}