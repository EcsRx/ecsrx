# Entity Collections

Most ECS systems have *something* that contains entities, be it `world`, `context`, `pool` etc. In EcsRx its called an `EntityCollection`.

## Why do we need them?

All the entities need to live somewhere, so where better than a form of collection. There is also some other benefits to this, as you can have entities split out over different collections, so for example if you had a schmup game where you had a few long living items and a LOT of bullets you may want to partition your bullet related entities separately to your long lived entities so you can access them easier and also constrain the chatter between groups.

### Partitioning?

So behind the scenes observable groups will subscribe to changes on entity collections, so any time a component changes on an entity in a collection, or an entity is added to a collection there will be an event raised (via an observable) which the observable groups will listen for.

So if you imagine you have 1000 entities and 10 observable groups all listening to a single collection, and you added a new component to each entity, that would fire 1000 events which would be picked up by 10 observable groups, so that in total is 10,000 invocations checks on observable groups for matching etc. So in some cases this is fine and you want all these observable groups to be notified of these changes, but in some cases you may want to partition your entities so only certain groups are notified of certain changes.

So taking the above example again, if you were to partition your collections so they are in different collections, you will reduce the chatter as the groups would be listening to separate collections, this can improve your performance a bit and make it easier to manage certain types of entities, i.e if you have lots of short lived entities which are constantly being re-created, better to keep that to a smaller collection which doesnt notify all groups.

> There is more information about how to make use of partitioned collections in the performance section under `system affinity`

## Where do they live?

So there is an `EntityDatabase` which acts as the container for all the entity collections 