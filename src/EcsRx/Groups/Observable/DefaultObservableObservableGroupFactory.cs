namespace EcsRx.Groups.Observable
{
    public class DefaultObservableObservableGroupFactory : IObservableGroupFactory
    {
        public IObservableGroup Create(ObservableGroupConfiguration arg)
        {
            return new ObservableGroup(arg.ObservableGroupToken, arg.InitialEntities, arg.NotifyingCollections);      
        }
    }
}