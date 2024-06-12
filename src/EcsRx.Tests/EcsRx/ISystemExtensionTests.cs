using System.Linq;
using EcsRx.Extensions;
using SystemsRx.Extensions;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.EcsRx
{
    public class ISystemExtensionTests
    {
        public interface MultipleInterfaces : IReactToDataSystem<int>, IReactToDataSystem<float>{}

        [Fact]
        public void should_identify_if_system_is_reactive_data_system()
        {
            var fakeSystem = Substitute.For<IReactToDataSystem<int>>();
            Assert.True(fakeSystem.IsReactiveDataSystem());
        }

        [Fact]
        public void should_get_interface_generic_type_from_reactive_data_system()
        {
            var fakeSystem = Substitute.For<IReactToDataSystem<int>>();
            var genericTypes = fakeSystem.GetGenericDataTypes(typeof(IReactToDataSystem<>)).ToArray();
            
            Assert.Equal(1, genericTypes.Length);
            Assert.Contains(typeof(int), genericTypes);
        }
        
        [Fact]
        public void should_get_interface_generic_types_from_multiple_reactive_data_system()
        {
            var fakeSystem = Substitute.For<MultipleInterfaces>();
            var genericTypes = fakeSystem.GetGenericDataTypes(typeof(IReactToDataSystem<>)).ToArray();
            
            Assert.Equal(2, genericTypes.Length);
            Assert.Contains(typeof(int), genericTypes);
            Assert.Contains(typeof(float), genericTypes);
        }
    }
}