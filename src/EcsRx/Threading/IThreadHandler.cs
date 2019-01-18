using System;

namespace EcsRx.Threading
{
    public interface IThreadHandler
    {
        void For(int start, int end, Action<int> process);
    }
}