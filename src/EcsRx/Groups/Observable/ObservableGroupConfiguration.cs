using System.Collections.Generic;
using EcsRx.Collections.Entity;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupConfiguration
    {
        public ObservableGroupToken ObservableGroupToken { get; set; }
        public IEnumerable<INotifyingCollection> NotifyingCollections { get; set; }
        public IEnumerable<IEntity> InitialEntities { get; set; }
    }
}