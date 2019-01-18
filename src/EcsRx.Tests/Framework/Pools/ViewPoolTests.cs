using System.Linq;
using EcsRx.Plugins.Views.Pooling;
using EcsRx.Plugins.Views.ViewHandlers;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework.Pools
{
    public class ViewPoolTests
    {
        [Fact]
        public void should_pre_allocate_views()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();
            mockViewHandler.CreateView().Returns(null);

            var pool = new ViewPool(5, mockViewHandler);
            pool.PreAllocate(20);

            mockViewHandler.Received(20).CreateView();
            Assert.Equal(20, pool.PooledObjects.Count);
            Assert.All(pool.PooledObjects, x => Assert.False(x.IsInUse));
            Assert.All(pool.PooledObjects, x => Assert.Null(x.ViewObject));
        }

        [Fact]
        public void should_only_deallocate_unsued_views()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();

            var pool = new ViewPool(5, mockViewHandler);
            for (var i = 0; i < 10; i++)
            {
                var viewObject = new ViewObjectContainer(null);

                if (i < 5)
                { viewObject.IsInUse = true; }

                pool.PooledObjects.Add(viewObject);
            }

            pool.DeAllocate(10);

            mockViewHandler.Received(5).DestroyView(Arg.Any<object>());
            Assert.Equal(5, pool.PooledObjects.Count);
            Assert.All(pool.PooledObjects, x => Assert.True(x.IsInUse));
        }

        [Fact]
        public void should_empty_pool()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();

            var pool = new ViewPool(5, mockViewHandler);
            for (var i = 0; i < 10; i++)
            {
                var viewObject = new ViewObjectContainer(null);

                if (i < 5)
                { viewObject.IsInUse = true; }

                pool.PooledObjects.Add(viewObject);
            }

            pool.EmptyPool();

            mockViewHandler.Received(10).DestroyView(Arg.Any<object>());
            Assert.Empty(pool.PooledObjects);
        }

        [Fact]
        public void should_allocate_in_bulk_when_needing_more_instances()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();
            var pool = new ViewPool(5, mockViewHandler);
            pool.AllocateInstance();

            mockViewHandler.Received(5).CreateView();
            Assert.Equal(5, pool.PooledObjects.Count);
            Assert.Equal(4, pool.PooledObjects.Count(x => x.IsInUse == false));
            Assert.Equal(1, pool.PooledObjects.Count(x => x.IsInUse));
        }

        [Fact]
        public void should_not_allocate_in_bulk_when_views_not_in_use()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();
            var pool = new ViewPool(5, mockViewHandler);

            var viewObject = new ViewObjectContainer(null);
            pool.PooledObjects.Add(viewObject);

            pool.AllocateInstance();

            mockViewHandler.Received(0).CreateView();
            mockViewHandler.Received(1).SetActiveState(Arg.Any<object>(), true);
            Assert.Equal(1, pool.PooledObjects.Count);
            Assert.Equal(1, pool.PooledObjects.Count(x => x.IsInUse));
        }

        [Fact]
        public void should_not_destroy_on_deallocation()
        {
            var mockViewHandler = Substitute.For<IViewHandler>();
            var pool = new ViewPool(5, mockViewHandler);

            var actualView = new object();
            var viewObject = new ViewObjectContainer(actualView) { IsInUse = true };
            pool.PooledObjects.Add(viewObject);

            pool.ReleaseInstance(actualView);

            mockViewHandler.Received(0).DestroyView(actualView);
            mockViewHandler.Received(1).SetActiveState(actualView, false);
            Assert.Equal(1, pool.PooledObjects.Count);
            Assert.Equal(1, pool.PooledObjects.Count(x => x.IsInUse == false));
        }
    }
}