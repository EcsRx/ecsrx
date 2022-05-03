using System;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups.Observable.Tracking.Events;
using EcsRx.Groups.Observable.Tracking.Types;
using SystemsRx.Extensions;
using SystemsRx.MicroRx.Disposables;
using SystemsRx.MicroRx.Extensions;
using SystemsRx.MicroRx.Subjects;

namespace EcsRx.Groups.Observable.Tracking.Trackers
{
    public class IndividualObservableGroupTracker : ObservableGroupTracker, IIndividualObservableGroupTracker
    {
        private CompositeDisposable _subs;
        public GroupMatchingType CurrentMatchingType { get; private set; }

        public IndividualObservableGroupTracker(LookupGroup lookupGroup, IEntity entity) : base(lookupGroup)
        {
            _subs = new CompositeDisposable();
            entity.ComponentsAdded.Subscribe(x => OnEntityComponentAdded(x, entity)).AddTo(_subs);
            entity.ComponentsRemoving.Subscribe(x => OnEntityComponentRemoving(x, entity)).AddTo(_subs);
            entity.ComponentsRemoved.Subscribe(x => OnEntityComponentRemoved(x, entity)).AddTo(_subs);
            CurrentMatchingType = LookupGroup.CalculateMatchingType(entity);
        }

        public bool IsMatching() => CurrentMatchingType == GroupMatchingType.MatchesNoExcludes;

        public override void UpdateState(int entityId, GroupMatchingType newMatchingType)
        { CurrentMatchingType = newMatchingType; }

        public override GroupMatchingType GetState(int entityId)
        { return CurrentMatchingType; }

        public override void Dispose()
        {
            OnGroupMatchingChanged?.Dispose();
            _subs.Dispose();
        }
    }
}