using EcsRx.Groups.Observable;
using Xunit;

namespace EcsRx.Tests.Framework.Observables
{
    public class ObservableGroupTokenTests
    {
        [Fact]
        public void should_correctly_differentiate_tokens()
        {
            var defaultRequired = new[] {1};
            var defaultExcluded = new int[0];
            var token1 = new ObservableGroupToken(defaultRequired, defaultExcluded);
            var token2 = new ObservableGroupToken(defaultRequired, defaultExcluded, 1);
            var token3 = new ObservableGroupToken(defaultRequired, defaultExcluded, 1, 2);
            
            Assert.NotEqual(token1, token2);
            Assert.NotEqual(token1, token3);
            Assert.NotEqual(token2, token3);
            
            Assert.NotEqual(token1.GetHashCode(), token2.GetHashCode());
            Assert.NotEqual(token1.GetHashCode(), token3.GetHashCode());
            Assert.NotEqual(token2.GetHashCode(), token3.GetHashCode());
            
            Assert.Equal(token1, token1);
            Assert.Equal(token2, token2);
            Assert.Equal(token3, token3);
            
            Assert.Equal(token1.GetHashCode(), token1.GetHashCode());
            Assert.Equal(token2.GetHashCode(), token2.GetHashCode());
            Assert.Equal(token3.GetHashCode(), token3.GetHashCode());
        }

    }
}