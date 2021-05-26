using System;

namespace EcsRx.MicroRx.Disposables
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}