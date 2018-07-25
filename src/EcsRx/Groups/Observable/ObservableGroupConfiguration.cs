using System.Collections.Generic;
using EcsRx.Collections;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupConfiguration
    {
        public ObservableGroupToken ObservableGroupToken { get; set; }
        public INotifyingEntityCollection NotifyingCollection { get; set; }
        public IEnumerable<IEntity> InitialEntities { get; set; }
    }
}