using System;
using EcsRx.Entities;

namespace EcsRx.Systems
{
    public interface IReactToEntitySystem : ISystem
    {
        IObservable<IEntity> ReactToEntity(IEntity entity);

        void Execute(IEntity entity);
    }
}