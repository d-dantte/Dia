using Axis.Dia.TypeConverter.Clr;
using Axis.Luna.Extensions;

namespace Axis.Dia.TypeConverter.Tests.Clr
{
    [TestClass]
    public class RecordConverterTests
    {
        [TestMethod]
        public void CanConvert()
        {
            var converter = new RecordConverter();

            Assert.IsTrue(converter.CanConvert(Core.DiaType.Record, typeof(object).ToTypeInfo()));
            Assert.IsTrue(converter.CanConvert(Core.DiaType.Record, typeof(Dictionary<string, int>).ToTypeInfo()));

            Assert.IsFalse(converter.CanConvert(Core.DiaType.Sequence, typeof(object).ToTypeInfo()));
            Assert.IsFalse(converter.CanConvert(Core.DiaType.Record, typeof(object[]).ToTypeInfo()));
        }

        [TestMethod]
        public void ToClr_Tests()
        {
            var options = Options.NewBuilder().Build();
            var context = new ConverterContext(options);
            var converter = new RecordConverter();
            var record = new Core.Types.Record
            {
                ["count"] = 5
            };

            var result = converter.ToClr(
                record,
                typeof(CustomObj).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out CustomObj _));

            result = converter.ToClr(
                record,
                typeof(IDictionary<string, int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out IDictionary<string, int> _));

            result = converter.ToClr(
                Core.Types.Record.Null(),
                typeof(IDictionary<string, int>).ToTypeInfo(),
                context);
            Assert.IsNull(result);

            Assert.ThrowsException<InvalidOperationException>(
                () => converter.ToClr(Core.Types.Integer.Null(), typeof(int).ToTypeInfo(), context));
        }

        #region Map
        [TestMethod]
        public void FilterNulls_Tests()
        {
            var ignoreNullsOptions = Options
                .NewBuilder()
                .WithIgnoreNullsForRecords(true)
                .Build();
            var admitNullsOptions = Options
                .NewBuilder()
                .WithIgnoreNullsForRecords(false)
                .Build();

            var nonNullProperty = Core.Types.Record.Property.Of("NonNull", 45);
            var nullProperty = Core.Types.Record.Property.Of("Null", Core.Types.Integer.Null());

            Assert.IsTrue(RecordConverter.FilterNulls(nullProperty, admitNullsOptions));
            Assert.IsTrue(RecordConverter.FilterNulls(nonNullProperty, admitNullsOptions));

            Assert.IsTrue(RecordConverter.FilterNulls(nonNullProperty, ignoreNullsOptions));
            Assert.IsFalse(RecordConverter.FilterNulls(nullProperty, ignoreNullsOptions));
        }

        [TestMethod]
        public void ToMapKey_Tests()
        {
            var includeAttributesOptions = Options
                .NewBuilder()
                .WithIncludePropertyAttributesInMapKeyForRecords(true)
                .Build();
            var excludeAttributesOptions = Options
                .NewBuilder()
                .WithIncludePropertyAttributesInMapKeyForRecords(false)
                .Build();

            var nameAndAttribute = Core.Types.Record.PropertyName.Of("Property", "att1", ("att2", "value"));
            var name = Core.Types.Record.PropertyName.Of("Property");

            var result = RecordConverter.ToMapKey(name, excludeAttributesOptions);
            Assert.AreEqual("Property", result);

            result = RecordConverter.ToMapKey(nameAndAttribute, excludeAttributesOptions);
            Assert.AreEqual("Property", result);

            result = RecordConverter.ToMapKey(name, includeAttributesOptions);
            Assert.AreEqual("Property", result);

            result = RecordConverter.ToMapKey(nameAndAttribute, includeAttributesOptions);
            Assert.AreEqual("Property[att1; att2:value;]", result);
        }

        [TestMethod]
        public void NewMap_Tests()
        {
            Assert.ThrowsException<InvalidOperationException>(
                () => RecordConverter.NewMap<int>(typeof(Guid)));

            Assert.IsInstanceOfType<Dictionary<string, int>>(
                RecordConverter.NewMap<int>(typeof(Dictionary<string, int>)));

            Assert.IsInstanceOfType<Dictionary<string, int>>(
                RecordConverter.NewMap<int>(typeof(IDictionary<string, int>)));

            Assert.IsInstanceOfType<CustomDictionary>(
                RecordConverter.NewMap<int>(typeof(CustomDictionary)));

            Assert.ThrowsException<InvalidOperationException>(
                () => RecordConverter.NewMap<int>(typeof(CustomDictionary2)));
        }

        [TestMethod]
        public void ToMap_Generic_Tests()
        {
            var options = Options.NewBuilder().Build();
            var context = new ConverterContext(options);
            var record = new Core.Types.Record
            {
                ["only"] = 5
            };

            Assert.ThrowsException<ArgumentException>(() => RecordConverter.ToMap<int>(
                Core.Types.Record.Empty(),
                typeof(CustomDictionary2).ToTypeInfo(),
                context));

            var result = RecordConverter.ToMap<int>(
                record,
                typeof(IDictionary<string, int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out Dictionary<string, int> dict));
            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.TryGetValue("only", out var value));
            Assert.AreEqual(5, value);

            // uses the cached value
            var result2 = RecordConverter.ToMap<int>(
                record,
                typeof(IDictionary<string, int>).ToTypeInfo(),
                context);
            Assert.IsTrue(object.ReferenceEquals(result, result2));
        }

        [TestMethod]
        public void ToMap_Tests()
        {
            var options = Options.NewBuilder().Build();
            var context = new ConverterContext(options);
            var record = new Core.Types.Record
            {
                ["only"] = 5
            };

            var result = RecordConverter.ToMap(
                record,
                typeof(IDictionary<string, int>).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out Dictionary<string, int> dict));
            Assert.AreEqual(1, dict.Count);
            Assert.IsTrue(dict.TryGetValue("only", out var value));
            Assert.AreEqual(5, value);
        }
        #endregion

        #region Record
        [TestMethod]
        public void NormalizePropertyName_Tests()
        {
            var result = RecordConverter.NormalizePropertyName(
                Options.PropertyNameSource.Dia,
                "Property_name-HERE");
            Assert.AreEqual("propertynamehere", result);
        }

        [TestMethod]
        public void ToRecord_Generic_Tests()
        {
            var options = Options.NewBuilder().Build();
            var context = new ConverterContext(options);
            var now = DateTimeOffset.Now;
            var record = new Core.Types.Record
            {
                ["only"] = true,
                ["Name"] = "my name",
                ["DateOfBirth"] = now
            };

            Assert.ThrowsException<ArgumentException>(
                () => RecordConverter.ToRecord<CustomObj>(record, typeof(int).ToTypeInfo(), context));

            var typeinfo = typeof(CustomObj).ToTypeInfo();
            var result = RecordConverter.ToRecord<CustomObj>(record, typeinfo, context);
            Assert.IsNotNull(result);
            Assert.AreEqual("my name", result.Name);
            Assert.AreEqual(now, result.Date_of_birth);

            // use cached value
            var result2 = RecordConverter.ToRecord<CustomObj>(record, typeinfo, context);
            Assert.IsTrue(object.ReferenceEquals(result, result2));

            // duplicate clr property
            typeinfo = typeof(CustomObj2).ToTypeInfo();
            Assert.ThrowsException<InvalidOperationException>(
                () => RecordConverter.ToRecord<CustomObj2>(record, typeinfo, new ConverterContext(options)));

            // duplicate dia property
            record["Date_Of_Birth"] = now;
            Assert.ThrowsException<InvalidOperationException>(
                () => RecordConverter.ToRecord<CustomObj>(record, typeinfo, new ConverterContext(options)));
        }

        [TestMethod]
        public void ToRecord_Tests()
        {
            var options = Options.NewBuilder().Build();
            var context = new ConverterContext(options);
            var record = new Core.Types.Record
            {
                ["count"] = 5
            };

            var result = RecordConverter.ToRecord(
                record,
                typeof(CustomObj).ToTypeInfo(),
                context);
            Assert.IsTrue(result.Is(out CustomObj obj));
            Assert.AreEqual(5, obj.Count);
        }
        #endregion

        #region nested types
        public class CustomDictionary : Dictionary<string, int> { }

        public class CustomDictionary2 : Dictionary<string, int>
        {
            public CustomDictionary2(int dummy) { }
        }

        public class CustomObj
        {
            public int Count { get; set; }
            public string? Name { get; set; }
            public DateTimeOffset Date_of_birth { get; set; }
        }

        public class CustomObj2
        {
            public int Count { get; set; }
            public float count { get; set; }
            public string? Name { get; set; }
            public DateTimeOffset Date_of_birth { get; set; }
        }
        #endregion
    }
}
