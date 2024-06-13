using R3;

namespace EcsRx.Examples.Extensions
{
    public class ValueChanges<T>
    {
        public T PreviousValue { get; }
        public T CurrentValue { get; }

        public ValueChanges(T previousValue, T currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }
    }

    public static class IObservableExtensions
    {
        public static Observable<ValueChanges<T>> WithValueChange<T>(this Observable<T> source)
        {
            return source.Scan(new ValueChanges<T>(default(T), default(T)),
                (acc, current) => new ValueChanges<T>(acc.CurrentValue, current));
        }
    }
}