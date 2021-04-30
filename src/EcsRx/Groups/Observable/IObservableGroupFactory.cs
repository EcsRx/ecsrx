using SystemsRx.Factories;

namespace EcsRx.Groups.Observable
{
    public interface IObservableGroupFactory : IFactory<ObservableGroupConfiguration, IObservableGroup> {}
}