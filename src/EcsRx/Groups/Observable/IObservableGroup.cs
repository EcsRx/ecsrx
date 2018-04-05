using System;
using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public interface IObservableGroup
    {
        ObservableGroupToken Token { get; }
        IReadOnlyCollection<IEntity> Entities { get; }
        
        IObservable<IEntity> OnEntityAdded { get; }
        IObservable<IEntity> OnEntityRemoved { get; }
    }
}