using System.Collections.Generic;
using System.Linq;
using SystemsRx.Extensions;
using EcsRx.Plugins.Views.ViewHandlers;

namespace EcsRx.Plugins.Views.Pooling
{
    public class ViewPool : IViewPool
    {
        public readonly IList<ViewObjectContainer> PooledObjects = new List<ViewObjectContainer>();
        
        public int IncrementSize { get; }
        public IViewHandler ViewHandler { get; }

        public ViewPool(int incrementSize, IViewHandler viewHandler)
        {
            IncrementSize = incrementSize;
            ViewHandler = viewHandler;
        }
        
        public void PreAllocate(int allocationCount)
        {
            for (var i = 0; i < allocationCount; i++)
            {
                var newInstance = ViewHandler.CreateView();
                ViewHandler.SetActiveState(newInstance, false);
                
                var objectContainer = new ViewObjectContainer(newInstance);
                PooledObjects.Add(objectContainer);
            }
        }

        public void DeAllocate(int dellocationCount)
        {
            PooledObjects.Where(x => !x.IsInUse)
                .Take(dellocationCount)
                .ToArray()
                .ForEachRun(OnDeallocateView);
        }

        private void OnDeallocateView(ViewObjectContainer x)
        {
            PooledObjects.Remove(x);
            ViewHandler.DestroyView(x.ViewObject);
        }

        public object AllocateInstance()
        {
            var availableViewObject = PooledObjects.FirstOrDefault(x => !x.IsInUse);
            if (availableViewObject == null)
            {
                PreAllocate(IncrementSize);
                availableViewObject = PooledObjects.First(x => !x.IsInUse);
            }

            availableViewObject.IsInUse = true;
            ViewHandler.SetActiveState(availableViewObject.ViewObject, true);
            return availableViewObject.ViewObject;
        }
        
        public void ReleaseInstance(object view)
        {
            var container = PooledObjects.FirstOrDefault(x => x.ViewObject == view);
            if(container == null) { return; }

            container.IsInUse = false;
            var viewObject = container.ViewObject;
            ViewHandler.SetActiveState(viewObject, false);
        }

        public void EmptyPool()
        {
            PooledObjects.ToArray()
                .ForEachRun(OnDeallocateView);

            PooledObjects.Clear();
        }
    }
}