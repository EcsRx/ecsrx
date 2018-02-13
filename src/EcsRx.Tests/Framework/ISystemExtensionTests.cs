using EcsRx.Extensions;
using EcsRx.Systems;
using NSubstitute;
using NUnit.Framework;

namespace EcsRx.Tests
{
    [TestFixture]
    public class ISystemExtensionTests
    {
        [Test]
        public void should_identify_if_system_is_reactive_data_system()
        {
            var fakeSystem = Substitute.For<IReactToDataSystem<int>>();
            Assert.IsTrue(fakeSystem.IsReactiveDataSystem());
        }

        [Test]
        public void should_get_interface_generic_type_from_reactive_data_system()
        {
            var fakeSystem = Substitute.For<IReactToDataSystem<int>>();
            var genericType = fakeSystem.GetGenericDataType();
            var typesMatch = genericType == typeof(int);
            Assert.That(typesMatch);
        }
    }
}