using System.Collections.Generic;
using System.Linq;
using EcsRx.Extensions;
using EcsRx.Views.ViewHandlers;

namespace EcsRx.Views.Pooling
{
    public abstract class ViewPool : IViewPool
    {
        private readonly IList<ViewObjectContainer> _pooledObjects = new List<ViewObjectContainer>();
        
        public int IncrementSize { get; }
        public IViewHandler ViewHandler { get; }
        
        protected ViewPool(int incrementSize, IViewHandler viewHandler)
        {
            IncrementSize = incrementSize;
            ViewHandler = viewHandler;
        }

        protected abstract void SetActiveState(object view, bool isActive);
        
        public void PreAllocate(int allocationCount)
        {
            for (var i = 0; i < allocationCount; i++)
            {
                var newInstance = ViewHandler.SetupView();
                SetActiveState(newInstance, false);
                var objectContainer = new ViewObjectContainer(newInstance);
                _pooledObjects.Add(objectContainer);
            }
        }

        public void DeAllocate(int dellocationCount)
        {
            _pooledObjects.Where(x => !x.IsInUse)
                .Take(dellocationCount)
                .ToArray()
                .ForEachRun(x =>
                {
                    _pooledObjects.Remove(x);
                    DestroyView(x.ViewObject);
                });
        }

        public object AllocateInstance()
        {
            var availableViewObject = _pooledObjects.FirstOrDefault(x => !x.IsInUse);
            if (availableViewObject == null)
            {
                PreAllocate(IncrementSize);
                availableViewObject = _pooledObjects.First(x => !x.IsInUse);
            }

            availableViewObject.IsInUse = true;
            return availableViewObject.ViewObject;
        }
        
        public void ReleaseInstance(object view)
        {
            var container = _pooledObjects.FirstOrDefault(x => x.ViewObject == view);
            if(container == null) { return; }

            container.IsInUse = false;
            var viewObject = container.ViewObject;
            SetActiveState(viewObject, false);
        }

        public void EmptyPool()
        { _pooledObjects.Clear(); }
    }
}