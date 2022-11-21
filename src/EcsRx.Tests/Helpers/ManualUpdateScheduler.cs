using System;
using System.Reactive.Subjects;
using SystemsRx.Scheduling;

namespace EcsRx.Tests.Helpers
{
    public class ManualUpdateScheduler : IUpdateScheduler
    {
        public ElapsedTime ElapsedTime { get; set; }
     
        public Subject<ElapsedTime> ElapsedTimeTrigger { get; }

        public ManualUpdateScheduler(Subject<ElapsedTime> elapsedTimeTrigger)
        { ElapsedTimeTrigger = elapsedTimeTrigger; }

        public void Dispose()
        { ElapsedTimeTrigger.Dispose(); }

        public IObservable<ElapsedTime> OnPreUpdate => ElapsedTimeTrigger;
        public IObservable<ElapsedTime> OnUpdate => ElapsedTimeTrigger;
        public IObservable<ElapsedTime> OnPostUpdate => ElapsedTimeTrigger;
    }
}