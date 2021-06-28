using System;
using System.Collections.Generic;
using System.Diagnostics;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Components.Lookups;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Builders
{
    public class ReferenceBatchBuilder<T1, T2> : IReferenceBatchBuilder<T1, T2> 
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2;

        public ReferenceBatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
        }

        public ReferenceBatch<T1, T2>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var batches = new ReferenceBatch<T1, T2>[entities.Count];

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                { return Array.Empty<ReferenceBatch<T1, T2>>(); }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                batches[i] = new ReferenceBatch<T1, T2>(entity.Id, componentArray1[component1Allocation], componentArray2[component2Allocation]);
            }
            
            return batches;
        }
    }
    
    public class ReferenceBatchBuilder<T1, T2, T3> : IReferenceBatchBuilder<T1, T2, T3> 
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3;

        public ReferenceBatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
        }

        public ReferenceBatch<T1, T2, T3>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            
            var batches = new ReferenceBatch<T1, T2, T3>[entities.Count];

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                { return Array.Empty<ReferenceBatch<T1, T2, T3>>(); }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                batches[i] = new ReferenceBatch<T1, T2, T3>(
                    entity.Id, componentArray1[component1Allocation], componentArray2[component2Allocation],
                    componentArray3[component3Allocation]);
            }
            
            return batches;
        }
    }
    
    public class ReferenceBatchBuilder<T1, T2, T3, T4> : IReferenceBatchBuilder<T1, T2, T3, T4> 
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, _componentTypeId4;

        public ReferenceBatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
        }

        public ReferenceBatch<T1, T2, T3, T4>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            
            var batches = new ReferenceBatch<T1, T2, T3, T4>[entities.Count];

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                { return Array.Empty<ReferenceBatch<T1, T2, T3, T4>>(); }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                batches[i] = new ReferenceBatch<T1, T2, T3, T4>(
                    entity.Id, componentArray1[component1Allocation], componentArray2[component2Allocation],
                    componentArray3[component3Allocation], componentArray4[component4Allocation]);
            }

            return batches;
        }
    }
    
    public class ReferenceBatchBuilder<T1, T2, T3, T4, T5> : IReferenceBatchBuilder<T1, T2, T3, T4, T5> 
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, 
            _componentTypeId4, _componentTypeId5;

        public ReferenceBatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
            _componentTypeId5 = componentTypeLookup.GetComponentType(typeof(T5));
        }

        public ReferenceBatch<T1, T2, T3, T4, T5>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            
            var batches = new ReferenceBatch<T1, T2, T3, T4, T5>[entities.Count];
            
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                { return Array.Empty<ReferenceBatch<T1, T2, T3, T4, T5>>(); }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                batches[i] = new ReferenceBatch<T1, T2, T3, T4, T5>(
                    entity.Id, componentArray1[component1Allocation], componentArray2[component2Allocation],
                    componentArray3[component3Allocation], componentArray4[component4Allocation],
                    componentArray5[component5Allocation]);
            }
        
            return batches;
        }
    }
    
    public class ReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> : IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> 
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        public IComponentDatabase ComponentDatabase { get; }
        
        private readonly int _componentTypeId1, _componentTypeId2, _componentTypeId3, 
            _componentTypeId4, _componentTypeId5, _componentTypeId6;

        public ReferenceBatchBuilder(IComponentDatabase componentDatabase, IComponentTypeLookup componentTypeLookup)
        {
            ComponentDatabase = componentDatabase;
            _componentTypeId1 = componentTypeLookup.GetComponentType(typeof(T1));
            _componentTypeId2 = componentTypeLookup.GetComponentType(typeof(T2));
            _componentTypeId3 = componentTypeLookup.GetComponentType(typeof(T3));
            _componentTypeId4 = componentTypeLookup.GetComponentType(typeof(T4));
            _componentTypeId5 = componentTypeLookup.GetComponentType(typeof(T5));
            _componentTypeId6 = componentTypeLookup.GetComponentType(typeof(T6));
        }

        public ReferenceBatch<T1, T2, T3, T4, T5, T6>[] Build(IReadOnlyList<IEntity> entities)
        {
            var componentArray1 = ComponentDatabase.GetComponents<T1>(_componentTypeId1);
            var componentArray2 = ComponentDatabase.GetComponents<T2>(_componentTypeId2);
            var componentArray3 = ComponentDatabase.GetComponents<T3>(_componentTypeId3);
            var componentArray4 = ComponentDatabase.GetComponents<T4>(_componentTypeId4);
            var componentArray5 = ComponentDatabase.GetComponents<T5>(_componentTypeId5);
            var componentArray6 = ComponentDatabase.GetComponents<T6>(_componentTypeId6);
            
            var batches = new ReferenceBatch<T1, T2, T3, T4, T5, T6>[entities.Count];

            for (var i = 0; i < entities.Count; i++)
            {
                if (entities.Count != batches.Length)
                { return Array.Empty<ReferenceBatch<T1, T2, T3, T4, T5, T6>>(); }
                
                var entity = entities[i];
                var component1Allocation = entity.ComponentAllocations[_componentTypeId1];
                var component2Allocation = entity.ComponentAllocations[_componentTypeId2];
                var component3Allocation = entity.ComponentAllocations[_componentTypeId3];
                var component4Allocation = entity.ComponentAllocations[_componentTypeId4];
                var component5Allocation = entity.ComponentAllocations[_componentTypeId5];
                var component6Allocation = entity.ComponentAllocations[_componentTypeId6];
                batches[i] = new ReferenceBatch<T1, T2, T3, T4, T5, T6>(
                    entity.Id, componentArray1[component1Allocation], componentArray2[component2Allocation],
                    componentArray3[component3Allocation], componentArray4[component4Allocation],
                    componentArray5[component5Allocation], componentArray6[component6Allocation]);
            }

            return batches;
        }
    }
}