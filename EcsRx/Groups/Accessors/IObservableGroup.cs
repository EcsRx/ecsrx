using System.Collections.Generic;
using System.Reactive.Subjects;
using EcsRx.Entities;

namespace EcsRx.Groups.Accessors
{
    public interface IObservableGroup
    {
        ObservableGroupToken Token { get; }
        IEnumerable<IEntity> Entities { get; }
        
        Subject<IEntity> OnEntityAdded { get; }
        Subject<IEntity> OnEntityRemoved { get; }
    }
}