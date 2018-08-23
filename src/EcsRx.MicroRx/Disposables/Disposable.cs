using System;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.MicroRx.Disposables
{
    public static class Disposable
    {
        public static readonly IDisposable Empty = EmptyDisposable.Singleton;
    }
}