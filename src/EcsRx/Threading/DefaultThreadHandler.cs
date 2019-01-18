using System;
using System.Threading.Tasks;

namespace EcsRx.Threading
{
    public class DefaultThreadHandler : IThreadHandler
    {
        public void For(int start, int end, Action<int> process) => Parallel.For(start, end, process);
    }
}