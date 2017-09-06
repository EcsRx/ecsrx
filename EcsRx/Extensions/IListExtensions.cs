using System.Collections.Generic;
using System.Linq;
using EcsRx.Entities;
using EcsRx.Executor;

namespace EcsRx.Extensions
{
    public static class IListExtensions
    {
        public static IEnumerable<SubscriptionToken> GetTokensFor(this IList<SubscriptionToken> subscriptionTokens, IEntity entity)
        { return subscriptionTokens.Where(x => x.AssociatedObject == entity); }

        public static void RemoveAllFrom<T>(this IList<T> list, IEnumerable<T> elementsToRemove)
        {
            foreach (var element in elementsToRemove)
            { list.Remove(element); }
        }
    }
}