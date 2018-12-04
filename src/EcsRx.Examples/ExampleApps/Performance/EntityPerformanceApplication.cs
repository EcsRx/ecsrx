using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EcsRx.Components;
using EcsRx.Components.Database;
using EcsRx.Entities;
using EcsRx.Examples.Application;
using EcsRx.Examples.ExampleApps.Performance.Components;
using EcsRx.Examples.ExampleApps.Performance.Components.Specific;
using EcsRx.Examples.ExampleApps.Performance.Helper;
using EcsRx.Extensions;
using EcsRx.Infrastructure.Extensions;

namespace EcsRx.Examples.ExampleApps.Performance
{
    public class EntityPerformanceApplication : EcsRxConsoleApplication
    {
        private IComponent[] _availableComponents;
        private Type[] _availableComponentTypes;
        private readonly RandomGroupFactory _groupFactory = new RandomGroupFactory();
        private static readonly int EntityCount = 100000;

        private List<IEntity> _entities;

        protected override void BindSystems()
        {}
        
        protected override void ApplicationStarted()
        {                       
            var componentNamespace = typeof(Component1).Namespace;
            _availableComponentTypes = _groupFactory.GetComponentTypes
                .Where(x => x.Namespace == componentNamespace)
                .ToArray();
            
            _availableComponents = _availableComponentTypes
                .Select(x => Activator.CreateInstance(x) as IComponent)
                .ToArray();

            var componentRepository = Container.Resolve<IComponentRepository>();
                        
            _entities = new List<IEntity>();
            for (var i = 0; i < EntityCount; i++)
            {
                var entity = new Entity(i, componentRepository);
                entity.AddComponents(_availableComponents);
                _entities.Add(entity);                
            }

            var timeTaken = ProcessEntities();

            Console.WriteLine($"Finished In: {timeTaken.Milliseconds}ms");
        }

        private TimeSpan ProcessEntities()
        {
            EntityCollectionManager.Collections.ForEachRun(x => x.RemoveAllEntities());
            GC.Collect();
            var timer = Stopwatch.StartNew();

            for (var i = 0; i < EntityCount; i++)
            { ProcessEntity(_entities[i]); }

            timer.Stop();
            return TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds);
        }
        
        public void ProcessEntity(IEntity entity)
        {
            // Called just to make sure the method runs
            bool ignore;
            
            if(entity.HasAllComponents(_availableComponentTypes))
            { ignore = true; }
            
            var component1 = entity.GetComponent<Component1>();
            var component2 = entity.GetComponent<Component2>();
            var component3 = entity.GetComponent<Component3>();
            var component4 = entity.GetComponent<Component4>();
            var component5 = entity.GetComponent<Component5>();
            var component6 = entity.GetComponent<Component6>();
            var component7 = entity.GetComponent<Component7>();
            var component8 = entity.GetComponent<Component8>();
            var component9 = entity.GetComponent<Component9>();
            var component10 = entity.GetComponent<Component10>();
            var component11 = entity.GetComponent<Component11>();
            var component12 = entity.GetComponent<Component12>();
            var component13 = entity.GetComponent<Component13>();
            var component14 = entity.GetComponent<Component14>();
            var component15 = entity.GetComponent<Component15>();
            var component16 = entity.GetComponent<Component16>();
            var component17 = entity.GetComponent<Component17>();
            var component18 = entity.GetComponent<Component18>();
            var component19 = entity.GetComponent<Component19>();
            var component20 = entity.GetComponent<Component20>();

            // Stop optimizing away the usages
            if(component1 == null) { }
            if(component2 == null) { }
            if(component3 == null) { }
            if(component4 == null) { }
            if(component5 == null) { }
            if(component6 == null) { }
            if(component7 == null) { }
            if(component8 == null) { }
            if(component9 == null) { }
            if(component10 == null) { }
            if(component11 == null) { }
            if(component12 == null) { }
            if(component13 == null) { }
            if(component14 == null) { }
            if(component15 == null) { }
            if(component16 == null) { }
            if(component17 == null) { }
            if(component18 == null) { }
            if(component19 == null) { }
            if(component20 == null) { }
        }
    }
}