using R3;
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

        public Observable<ElapsedTime> OnPreUpdate => ElapsedTimeTrigger;
        public Observable<ElapsedTime> OnUpdate => ElapsedTimeTrigger;
        public Observable<ElapsedTime> OnPostUpdate => ElapsedTimeTrigger;
    }
}