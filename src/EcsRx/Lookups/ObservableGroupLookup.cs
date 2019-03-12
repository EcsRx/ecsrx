using System.Collections.ObjectModel;
using EcsRx.Groups.Observable;

namespace EcsRx.Lookups
{
    public class ObservableGroupLookup : KeyedCollection<ObservableGroupToken, IObservableGroup>
    {
        protected override ObservableGroupToken GetKeyForItem(IObservableGroup item) => item.Token;

        public IObservableGroup GetByIndex(int index) => Items[index];
    }
}