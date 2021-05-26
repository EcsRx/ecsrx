using System;
using System.Reactive;
using System.Reactive.Linq;
using EcsRx.MicroRx.Subjects;
using EcsRx.ReactiveData;
using EcsRx.ReactiveData.Extensions;
using EcsRx.Tests.Plugins.ReactiveData.Utils;
using Xunit;

namespace EcsRx.Tests.Plugins.ReactiveData
{
    public class ReactivePropertyTest
    {
        [Fact]
        public void ValueType()
        {
            {
                var rp = new ReactiveProperty<int>(); // 0

                var result = rp.Record();
                result.Values.Is(0);

                rp.Value = 0;
                result.Values.Is(0);

                rp.Value = 10;
                result.Values.Is(0, 10);

                rp.Value = 100;
                result.Values.Is(0, 10, 100);

                rp.Value = 100;
                result.Values.Is(0, 10, 100);
            }
            {
                var rp = new ReactiveProperty<int>(20);

                var result = rp.Record();
                result.Values.Is(20);

                rp.Value = 0;
                result.Values.Is(20, 0);

                rp.Value = 10;
                result.Values.Is(20, 0, 10);

                rp.Value = 100;
                result.Values.Is(20, 0, 10, 100);

                rp.Value = 100;
                result.Values.Is(20, 0, 10, 100);
            }
        }

        [Fact]
        public void ClassType()
        {
            {
                var rp = new ReactiveProperty<string>(); // null

                var result = rp.Record();
                result.Values.Is((string)null);

                rp.Value = null;
                result.Values.Is((string)null);

                rp.Value = "a";
                result.Values.Is((string)null, "a");

                rp.Value = "b";
                result.Values.Is((string)null, "a", "b");

                rp.Value = "b";
                result.Values.Is((string)null, "a", "b");
            }
            {
                var rp = new ReactiveProperty<string>("z");

                var result = rp.Record();
                result.Values.Is("z");

                rp.Value = "z";
                result.Values.Is("z");

                rp.Value = "a";
                result.Values.Is("z", "a");

                rp.Value = "b";
                result.Values.Is("z", "a", "b");

                rp.Value = "b";
                result.Values.Is("z", "a", "b");

                rp.Value = null;
                result.Values.Is("z", "a", "b", null);
            }
        }

        [Fact]
        public void ToReadOnlyReactivePropertyValueType()
        {
            {
                var source = new Subject<int>();
                var rp = source.ToReactiveProperty();

                var result = rp.Record();
                result.Values.Count.Is(0);

                source.OnNext(0);
                result.Values.Is(0);

                source.OnNext(10);
                result.Values.Is(0, 10);

                source.OnNext(100);
                result.Values.Is(0, 10, 100);

                source.OnNext(100);
                result.Values.Is(0, 10, 100);
            }
            {
                var source = new Subject<int>();
                var rp = source.ToReactiveProperty(0);

                var result = rp.Record();
                result.Values.Is(0);

                source.OnNext(0);
                result.Values.Is(0);

                source.OnNext(10);
                result.Values.Is(0, 10);

                source.OnNext(100);
                result.Values.Is(0, 10, 100);

                source.OnNext(100);
                result.Values.Is(0, 10, 100);
            }
        }



        [Fact]
        public void ToReactivePropertyClassType()
        {
            {
                var source = new Subject<string>();
                var rp = source.ToReactiveProperty();

                var result = rp.Record();
                result.Values.Count.Is(0);

                source.OnNext(null);
                result.Values.Is((string)null);

                source.OnNext("a");
                result.Values.Is((string)null, "a");

                source.OnNext("b");
                result.Values.Is((string)null, "a", "b");

                source.OnNext("b");
                result.Values.Is((string)null, "a", "b");
            }
            {
                var source = new Subject<string>();
                var rp = source.ToReactiveProperty("z");

                var result = rp.Record();
                result.Values.Is("z");

                source.OnNext("z");
                result.Values.Is("z");

                source.OnNext("a");
                result.Values.Is("z", "a");

                source.OnNext("b");
                result.Values.Is("z", "a", "b");

                source.OnNext("b");
                result.Values.Is("z", "a", "b");

                source.OnNext(null);
                result.Values.Is("z", "a", "b", null);

                source.OnNext(null);
                result.Values.Is("z", "a", "b", null);
            }
        }

        [Fact]
        public void FinishedSourceToReactiveProperty()
        {
            // pattern of OnCompleted
            /*
            {
                var source = Observable.Return(9);
                var rxProp = source.ToReactiveProperty();

                var notifications = rxProp.Record().Notifications;
                notifications.Is(Notification.CreateOnNext(9), Notification.CreateOnCompleted<int>());
                
                rxProp.Record().Notifications.Is(Notification.CreateOnNext(9), Notification.CreateOnCompleted<int>());
            }*/

            // pattern of OnError
            {
                // after
                {
                    var ex = new Exception();
                    var source = Observable.Throw<int>(ex);
                    var rxProp = source.ToReactiveProperty();

                    var notifications = rxProp.Record().Notifications;
                    notifications.Is(Notification.CreateOnError<int>(ex));

                    rxProp.Record().Notifications.Is(Notification.CreateOnError<int>(ex));
                }
                // immediate
                {

                    // var ex = new Exception();
                    var source = new Subject<int>();
                    var rxProp = source.ToReactiveProperty();

                    var record = rxProp.Record();

                    source.OnError(new Exception());

                    var notifications = record.Notifications;
                    notifications.Count.Is(1);
                    notifications[0].Kind.Is(NotificationKind.OnError);

                    rxProp.Record().Notifications[0].Kind.Is(NotificationKind.OnError);
                }
            }
        }

        [Fact(Skip = "No idea why this one doesnt work")]
        public void WithLastTest()
        {
            var rp1 = Observable.Return("1").ToReactiveProperty();
            rp1.LastAsync().Record().Notifications.Is(
                Notification.CreateOnNext("1"),
                Notification.CreateOnCompleted<string>());
        }
    }
}