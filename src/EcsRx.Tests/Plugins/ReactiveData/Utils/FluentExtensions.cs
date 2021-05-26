using Xunit;

namespace EcsRx.Tests.Plugins.ReactiveData.Utils
{
    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// no namespace.

[System.Diagnostics.DebuggerStepThroughAttribute]
public static partial class ChainingAssertion
{
    /// <summary>Assert.AreEqual, if T is IEnumerable then CollectionAssert.AreEqual</summary>
    public static void Is<T>(this T actual, T expected, string message = "")
    {
        if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            ((IEnumerable)actual).Cast<object>().Is(((IEnumerable)expected).Cast<object>(), message);
            return;
        }
        Assert.Equal(expected, actual);
    }

    /// <summary>Assert.IsTrue(expected(actual)).</summary>
    public static void Is<T>(this T actual, Func<T, bool> expected, string message = "")
    {
        Assert.True(expected(actual), message);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, params T[] expected)
    {
        var actualArray = actual.ToArray();
        Assert.Equal(expected, actualArray);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, string message = "")
    {
        Assert.Equal(expected.ToArray(), actual.ToArray());
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, IEqualityComparer<T> comparer, string message = "")
    {
        Is(actual, expected, comparer.Equals, message);
    }

    /// <summary>CollectionAssert.AreEqual</summary>
    public static void Is<T>(this IEnumerable<T> actual, IEnumerable<T> expected, Func<T, T, bool> equalityComparison, string message = "")
    {
        Assert.Equal(expected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison));
    }

    /// <summary>Assert.AreNotEqual, if T is IEnumerable then CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this T actual, T notExpected, string message = "")
    {
        if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
        {
            ((IEnumerable)actual).Cast<object>().IsNot(((IEnumerable)notExpected).Cast<object>(), message);
            return;
        }

        Assert.NotEqual(notExpected, actual);
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, params T[] notExpected)
    {
        IsNot(actual, notExpected.AsEnumerable());
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, string message = "")
    {
        Assert.NotEqual(notExpected.ToArray(), actual.ToArray());
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, IEqualityComparer<T> comparer, string message = "")
    {
        IsNot(actual, notExpected, comparer.Equals, message);
    }

    /// <summary>CollectionAssert.AreNotEqual</summary>
    public static void IsNot<T>(this IEnumerable<T> actual, IEnumerable<T> notExpected, Func<T, T, bool> equalityComparison, string message = "")
    {
        Assert.NotEqual(notExpected.ToArray(), actual.ToArray(), new ComparisonComparer<T>(equalityComparison));
    }

    /// <summary>Assert.IsNull</summary>
    public static void IsNull<T>(this T value, string message = "")
        where T : class
    {
        Assert.Null(value);
    }

    /// <summary>Assert.IsNotNull</summary>
    public static void IsNotNull<T>(this T value, string message = "")
        where T : class
    {
        Assert.NotNull(value);
    }

    /// <summary>Assert.IsTrue</summary>
    public static void IsTrue(this bool value, string message = "")
    {
        Assert.True(value);
    }

    /// <summary>Assert.IsFalse</summary>
    public static void IsFalse(this bool value, string message = "")
    {
        Assert.False(value);
    }


    static EqualInfo SequenceEqual(IEnumerable leftEnumerable, IEnumerable rightEnumarable, IEnumerable<string> names)
    {
        var le = leftEnumerable.GetEnumerator();
        using (le as IDisposable)
        {
            var re = rightEnumarable.GetEnumerator();

            using (re as IDisposable)
            {
                var index = 0;
                while (true)
                {
                    object lValue = null;
                    object rValue = null;
                    var lMove = le.MoveNext();
                    var rMove = re.MoveNext();
                    if (lMove) lValue = le.Current;
                    if (rMove) rValue = re.Current;

                    if (lMove && rMove)
                    {
                        var result = StructuralEqual(lValue, rValue, names.Concat(new[] { "[" + index + "]" }));
                        if (!result.IsEquals)
                        {
                            return result;
                        }
                    }

                    if ((lMove == true && rMove == false) || (lMove == false && rMove == true))
                    {
                        return new EqualInfo { IsEquals = false, Left = lValue, Right = rValue, Names = names.Concat(new[] { "[" + index + "]" }) };
                    }
                    if (lMove == false && rMove == false) break;
                    index++;
                }
            }
        }
        return new EqualInfo { IsEquals = true, Left = leftEnumerable, Right = rightEnumarable, Names = names };
    }

    static EqualInfo StructuralEqual(object left, object right, IEnumerable<string> names)
    {
        // type and basic checks
        if (object.ReferenceEquals(left, right)) return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
        if (left == null || right == null) return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };
        var lType = left.GetType();
        var rType = right.GetType();
        if (lType != rType) return new EqualInfo { IsEquals = false, Left = left, Right = right, Names = names };

        var type = left.GetType();

        // not object(int, string, etc...)
        if (Type.GetTypeCode(type) != TypeCode.Object)
        {
            return new EqualInfo { IsEquals = left.Equals(right), Left = left, Right = right, Names = names };
        }

        // is sequence
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            return SequenceEqual((IEnumerable)left, (IEnumerable)right, names);
        }

        // IEquatable<T>
        var equatable = typeof(IEquatable<>).MakeGenericType(type);
        if (equatable.IsAssignableFrom(type))
        {
            var result = (bool)equatable.GetMethod("Equals").Invoke(left, new[] { right });
            return new EqualInfo { IsEquals = result, Left = left, Right = right, Names = names };
        }

        // is object
        var fields = left.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        var properties = left.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetGetMethod(false) != null);
        var members = fields.Cast<MemberInfo>().Concat(properties.Cast<MemberInfo>());

        foreach (var mi in members)
        {
            var concatNames = names.Concat(new[] { (string)mi.Name });

            object lv;
            object rv;

            if (mi is FieldInfo)
            {
                var i = mi as FieldInfo;
                lv = i.GetValue(left);
                rv = i.GetValue(right);
            }
            else
            {
                var i = mi as PropertyInfo;
                lv = i.GetValue(left, null);
                rv = i.GetValue(right, null);
            }

            var result = StructuralEqual(lv, rv, concatNames);
            if (!result.IsEquals)
            {
                return result;
            }
        }

        return new EqualInfo { IsEquals = true, Left = left, Right = right, Names = names };
    }

    /// <summary>EqualityComparison to IComparer Converter for CollectionAssert</summary>
    private class ComparisonComparer<T> : IEqualityComparer<T>, IComparer, IComparer<T>
    {
        readonly Func<T, T, bool> comparison;

        public ComparisonComparer(Func<T, T, bool> comparison)
        {
            this.comparison = comparison;
        }

        public int Compare(object x, object y)
        {
            return (comparison != null)
                ? comparison((T)x, (T)y) ? 0 : -1
                : object.Equals(x, y) ? 0 : -1;
        }

        public int Compare(T x, T y)
        {
            return Compare(x, y);
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(T obj)
        {
            return 1;
        }
    }

    private class EqualInfo
    {
        public bool IsEquals;
        public object Left;
        public IEnumerable<string> Names;
        public object Right;
    }
}
}