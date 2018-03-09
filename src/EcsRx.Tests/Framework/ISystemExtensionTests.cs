using EcsRx.Extensions;
using EcsRx.Systems;
using NSubstitute;
using Xunit;

namespace EcsRx.Tests.Framework
{
    public class ISystemExtensionTests
    {
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
            var genericType = fakeSystem.GetGenericDataType();
            var typesMatch = genericType == typeof(int);
            Assert.True(typesMatch);
        }
    }
}