using System;
using EcsRx.Factories;

namespace EcsRx.Entities
{
    public interface IEntityFactory : IFactory<Guid?, IEntity> {}
}