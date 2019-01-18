using System.Collections.Generic;
using EcsRx.Components;
using EcsRx.Entities;
using EcsRx.Plugins.Batching.Batches;

namespace EcsRx.Plugins.Batching.Builders
{
    public interface IReferenceBatchBuilder : IBatchBuilder {}
    
    public interface IReferenceBatchBuilder<T1, T2> : IReferenceBatchBuilder
        where T1 : class, IComponent
        where T2 : class, IComponent
    {
        ReferenceBatch<T1, T2>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IReferenceBatchBuilder<T1, T2, T3> : IReferenceBatchBuilder
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IReferenceBatchBuilder<T1, T2, T3, T4> : IReferenceBatchBuilder
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IReferenceBatchBuilder<T1, T2, T3, T4, T5> : IReferenceBatchBuilder
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4, T5>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    public interface IReferenceBatchBuilder<T1, T2, T3, T4, T5, T6> : IReferenceBatchBuilder
        where T1 : class, IComponent
        where T2 : class, IComponent
        where T3 : class, IComponent
        where T4 : class, IComponent
        where T5 : class, IComponent
        where T6 : class, IComponent
    {
        ReferenceBatch<T1, T2, T3, T4, T5, T6>[] Build(IReadOnlyList<IEntity> entities);
    }
    
    
}