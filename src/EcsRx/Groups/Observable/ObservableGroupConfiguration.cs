using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Collections.Entity;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupConfiguration
    {
        public ObservableGroupToken ObservableGroupToken { get; set; }
        public IEnumerable<INotifyingEntityCollection> NotifyingCollections { get; set; }
        public IEnumerable<IEntity> InitialEntities { get; set; }
    }
}