using System;

namespace EcsRx.Scheduling
{
    public struct ElapsedTime
    {
        public TimeSpan DeltaTime;
        public TimeSpan TotalTime;
        
        public ElapsedTime(TimeSpan deltaTime, TimeSpan totalTime)
        {
            DeltaTime = deltaTime;
            TotalTime = totalTime;
        }
    }
}