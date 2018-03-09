using System.Collections.Generic;
using EcsRx.Entities;

namespace EcsRx.Groups.Observable
{
    public class ObservableGroupConfiguration
    {
        public ObservableGroupToken ObservableGroupToken { get; set; }
        public IEnumerable<IEntity> InitialEntities { get; set; }
    }
}