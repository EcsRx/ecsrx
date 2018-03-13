using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Groups.Observable;

namespace EcsRx.Collections
{
    public interface IEntityCollectionManager
    {
        IEnumerable<IEntityCollection> Pools { get; }

        IEnumerable<IEntity> GetEntitiesFor(IGroup group, string collectionName = null);
        IObservableGroup CreateObservableGroup(IGroup group, string collectionName = null);

        IEntityCollection CreateCollection(string name);
        IEntityCollection GetCollection(string name = null);
        void RemoveCollection(string name);
    }
}