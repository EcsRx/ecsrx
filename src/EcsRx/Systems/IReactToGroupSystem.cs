using System;
using EcsRx.Entities;
using EcsRx.Groups.Observable;

namespace EcsRx.Systems
{
    public interface IReactToGroupSystem : ISystem
    {
        IObservable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup);
        void Execute(IEntity entity);
    }
}