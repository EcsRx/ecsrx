using System.Collections.Generic;
using EcsRx.Entities;
using EcsRx.Pools;

namespace EcsRx.Groups.Accessors
{
    public class ObservableGroup : IObservableGroup
    {
        public ObservableGroupToken Token { get; }
        public IEnumerable<IEntity> Entities { get; }

        public ObservableGroup(ObservableGroupToken token, IEnumerable<IEntity> entities)
        {
            Token = token;
            Entities = entities;
        }
    }
}