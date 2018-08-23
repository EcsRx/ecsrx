using System;

/*
 *    This code was taken from UniRx project by neuecc
 *    https://github.com/neuecc/UniRx
 */
namespace EcsRx.MicroRx
{
    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => throw ex;
    }

    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = (T t) => { };
    }
}