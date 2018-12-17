using System;
using EcsRx.Tests.Computeds;
using Xunit;

namespace EcsRx.Tests.Framework.Observables
{
    public class ComputedFromDataTests
    {
        [Fact]
        public void should_populate_on_creation()
        {
            var expectedData = 10;
            var data = new DummyData{Data = expectedData};            
            
            var computedData = new TestComputedFromData(data);
            Assert.Equal(expectedData, computedData.CachedData);
        }
        
        [Fact]
        public void should_refresh_value_when_changed_and_value_requested()
        {
            var expectedData = 20;
            var data = new DummyData{Data = 10};
            
            var computedData = new TestComputedFromData(data);

            data.Data = expectedData;
            computedData.ManuallyRefresh.OnNext(true);

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
        }
        
        [Fact]
        public void should_not_refresh_value_when_not_changed_and_value_requested()
        {
            var expectedData = 20;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);

            data.Data = 10;

            var actualData = computedData.Value;
            Assert.Equal(expectedData, actualData);      
        }
        
        [Fact]
        public void should_not_refresh_data_when_changed_but_no_subs_or_value_requests()
        {
            var expectedData = 10;
            var data = new DummyData{Data = expectedData};
            
            var computedData = new TestComputedFromData(data);

            data.Data = 20;
            computedData.ManuallyRefresh.OnNext(true);
            
            Assert.Equal(expectedData, computedData.CachedData);      
        }        
        
        [Fact]
        public void should_refresh_data_when_changed_with_subs()
        {
            var expectedData = 10;
            var data = new DummyData{Data = 20};
            
            var computedData = new TestComputedFromData(data);
            computedData.Subscribe(x => { });
            
            data.Data = expectedData;
            computedData.ManuallyRefresh.OnNext(true);
            
            Assert.Equal(expectedData, computedData.CachedData);      
        }    
    }
}