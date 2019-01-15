using EcsRx.Groups.Observable;

namespace EcsRx.Plugins.Batching.Accessors
{
    public class AccessorToken
    {
        public int[] ComponentTypeIds { get; }
        public IObservableGroup ObservableGroup { get; }

        public AccessorToken(int[] componentTypeIds, IObservableGroup observableGroup)
        {
            ComponentTypeIds = componentTypeIds;
            ObservableGroup = observableGroup;
        }
    }
}