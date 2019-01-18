using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Builders
{
    public interface IBatchBuilder {}
    
    public interface IBatchBuilder<T1, T2> : IBatchBuilder
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
    {
        Batch<T1, T2>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IBatchBuilder<T1, T2, T3> : IBatchBuilder
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
    {
        Batch<T1, T2, T3>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IBatchBuilder<T1, T2, T3, T4> : IBatchBuilder
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IBatchBuilder<T1, T2, T3, T4, T5> : IBatchBuilder
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IBatchBuilder<T1, T2, T3, T4, T5, T6> : IBatchBuilder
        where T1 : unmanaged, IComponent
        where T2 : unmanaged, IComponent
        where T3 : unmanaged, IComponent
        where T4 : unmanaged, IComponent
        where T5 : unmanaged, IComponent
        where T6 : unmanaged, IComponent
    {
        Batch<T1, T2, T3, T4, T5, T6>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    
}