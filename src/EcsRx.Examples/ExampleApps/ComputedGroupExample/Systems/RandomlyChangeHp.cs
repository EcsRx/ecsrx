using System;
using EcsRx.Entities;
using EcsRx.Examples.ExampleApps.ComputedGroupExample.Components;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using R3;

namespace EcsRx.Examples.ExampleApps.ComputedGroupExample.Systems
{
    public class RandomlyChangeHpGroupSystem : IReactToGroupSystem
    {
        private const int HealthChange = 20;
        
        public IGroup Group { get; } = new Group(typeof(HasHealthComponent));
        private Random _random = new Random();
        
        public Observable<IObservableGroup> ReactToGroup(IObservableGroup observableGroup)
        { return Observable.Interval(TimeSpan.FromMilliseconds(500)).Select(x => observableGroup); }

        public void Process(IEntity entity)
        {
            var healthComponent = entity.GetComponent<HasHealthComponent>();

            var healthChange = CreateRandomHealthChange();
            healthComponent.CurrentHealth += healthChange;

            if (healthComponent.CurrentHealth <= 0 || healthComponent.CurrentHealth > healthComponent.MaxHealth)
            { healthComponent.CurrentHealth = healthComponent.MaxHealth; }            
        }

        public int CreateRandomHealthChange()
        { return _random.Next(-HealthChange, 0); }
    }
}