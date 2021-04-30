using System;

namespace SystemsRx.Scheduling
{
    public struct ElapsedTime : IEquatable<ElapsedTime>
    {
        public TimeSpan DeltaTime;
        public TimeSpan TotalTime;
        
        public ElapsedTime(TimeSpan deltaTime, TimeSpan totalTime)
        {
            DeltaTime = deltaTime;
            TotalTime = totalTime;
        }
        
        public bool Equals(ElapsedTime other)
        {
            return DeltaTime.Equals(other.DeltaTime) && TotalTime.Equals(other.TotalTime);
        }

        public override bool Equals(object obj)
        {
            return obj is ElapsedTime other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DeltaTime.GetHashCode() * 397) ^ TotalTime.GetHashCode();
            }
        }

    }
}