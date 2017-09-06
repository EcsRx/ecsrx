using System;
using System.Reactive.Subjects;
using EcsRx.Entities;

namespace EcsRx.Groups.Watchers
{
    public interface IGroupWatcher : IDisposable
    {
        Type[] ComponentTypes { get; }

        Subject<IEntity> OnEntityAdded { get; }
        Subject<IEntity> OnEntityRemoved { get; }
    }
}