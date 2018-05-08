using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public interface IObservableGroup : IEnumerable<IEntity>
    {
        ObservableGroupToken Token { get; }
        
        IObservable<IEntity> OnEntityAdded { get; }
        IObservable<IEntity> OnEntityRemoved { get; }
    }
}