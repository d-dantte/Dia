using Axis.Dia.Convert.Type.Clr;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;

namespace Axis.Dia.Tests.Convert.Type.Clr
{
    [TestClass]
    public class ReferenceTrackerTests
    {
        [TestMethod]
        public void TrackConversion_Test()
        {
            var tracker = new ReferenceTracker();
            var record = new RecordValue
            {
                ["stuff"] = 56
            };
            var recordRef = ReferenceValue.Of(record);

            // null reference
            Assert.ThrowsException<ArgumentNullException>(() => tracker.TrackConversion(
                null, () => Result.Of((object?)null)));

            // unlinked reference
            Assert.ThrowsException<ArgumentException>(() => tracker.TrackConversion(
                ReferenceValue.Of(Guid.NewGuid()), () => Result.Of((object?)null)));

            // null converter
            Assert.ThrowsException<ArgumentNullException>(() => tracker.TrackConversion(recordRef, null));

            // valid tracking
            var result = tracker.TrackConversion(recordRef, () =>  Result.Of((object?)"not stuff"));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsDataResult());
            Assert.AreEqual("not stuff", result.Resolve());

            // value was tracked
            var isTracked = tracker.IsTracked(recordRef, out var info);
            Assert.IsTrue(isTracked);
            Assert.IsInstanceOfType<TrackingInfo.Value>(info);
            Assert.AreEqual("not stuff", info.As<TrackingInfo.Value>().TrackedValue);

            // reference already tracked
            Assert.IsInstanceOfType<InvalidOperationException>(tracker
                .TrackConversion(recordRef, () => Result.Of((object?)"not stuff"))
                .AsError()
                .Cause()
                .InnerException);


            // ensure apply place holder is called
            tracker = new ReferenceTracker();
            var callCount = 0;
            var isRegistered = false;
            result = tracker.TrackConversion(recordRef, () =>
            {
                isRegistered = tracker.TryRegisterLazyReferenceInitializer(recordRef, v => callCount++);

                return Result.Of((object?)"not stuff");
            });
            Assert.IsTrue(isRegistered);
            Assert.AreEqual(1, callCount);

            // ensure mapping is removed on erroneous conversion
            tracker = new ReferenceTracker();
            result = tracker.TrackConversion(recordRef, () =>
            {
                return Result.Of<object?>(new Exception());
            });

            Assert.IsFalse(tracker.IsTracked(recordRef, out _));

            // ensure mapping is removed on erroneous call back
            tracker = new ReferenceTracker();
            result = tracker.TrackConversion(recordRef, () =>
            {
                isRegistered = tracker.TryRegisterLazyReferenceInitializer(recordRef, v => throw new Exception());

                return Result.Of((object?)"not stuff");
            });

            Assert.IsFalse(tracker.IsTracked(recordRef, out _));
        }

        [TestMethod]
        public void IsTracked_Tests()
        {
            var tracker = new ReferenceTracker();
            var record = new RecordValue
            {
                ["stuff"] = 56
            };
            var recordRef = ReferenceValue.Of(record);

            // null ref
            Assert.ThrowsException<ArgumentNullException>(() => tracker.IsTracked(null, out _));

            // unlinked ref
            Assert.ThrowsException<ArgumentException>(() => tracker.IsTracked(ReferenceValue.Of(Guid.NewGuid()), out _));

            // valid untracked ref
            var isTracked = tracker.IsTracked(recordRef, out _);
            Assert.IsFalse(isTracked);

            // valid tracking
            _ = tracker.TrackConversion(recordRef, () => Result.Of((object?)"not stuff"));
            isTracked = tracker.IsTracked(recordRef, out _);
            Assert.IsTrue(isTracked);
        }

        [TestMethod]
        public void TryRegisterLazyReferenceInitializer_Tests()
        {
            var tracker = new ReferenceTracker();
            var record = new RecordValue
            {
                ["stuff"] = 56
            };
            var recordRef = ReferenceValue.Of(record);

            // non reference value
            var isRegistered = tracker.TryRegisterLazyReferenceInitializer(null, v => { });
            Assert.IsFalse(isRegistered);

            isRegistered = tracker.TryRegisterLazyReferenceInitializer(IntValue.Of(3), v => { });
            Assert.IsFalse(isRegistered);

            // unlinked ref
            isRegistered = tracker.TryRegisterLazyReferenceInitializer(ReferenceValue.Of(Guid.NewGuid()), v => { });
            Assert.IsFalse(isRegistered);

            // untracked ref
            isRegistered = tracker.TryRegisterLazyReferenceInitializer(recordRef, v => { });
            Assert.IsFalse(isRegistered);

            // fully tracked ref
            tracker.TrackConversion(recordRef, () => Result.Of<object?>(43));
            isRegistered = tracker.TryRegisterLazyReferenceInitializer(recordRef, v => { });
            Assert.IsFalse(isRegistered);

            // partially tracked ref
            tracker = new ReferenceTracker();
            tracker.TrackConversion(recordRef, () =>
            {
                isRegistered = tracker.TryRegisterLazyReferenceInitializer(recordRef, v => { });
                return Result.Of<object?>(43);
            });
            Assert.IsTrue(isRegistered);
        }
    }

    [TestClass]
    public class PlaceholderTsts
    {
        [TestMethod]
        public void General_Tests()
        {
            var placeholder = new TrackingInfo.Placeholder();

            // null call back
            Assert.ThrowsException<ArgumentNullException>(() => placeholder.RegisterAssignmentCallback(null));

            // valid call back
            var callCount = 0;
            var placeholder2 = placeholder.RegisterAssignmentCallback(v => callCount++);
            Assert.AreEqual(placeholder2, placeholder);
            Assert.AreEqual(1, placeholder.CallbackCount);

            // apply
            placeholder.ApplyValue(new object());

            Assert.AreEqual(1, callCount);

            Assert.ThrowsException<InvalidOperationException>(() => placeholder.RegisterAssignmentCallback(v => { }));
        }
    }
}
