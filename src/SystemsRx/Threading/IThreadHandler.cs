using System;

namespace SystemsRx.Threading
{
    public interface IThreadHandler
    {
        void For(int start, int end, Action<int> process);
    }
}