using Axis.Dia.Core.Types;
using Axis.Dia.Json.Deserializers;
using Newtonsoft.Json.Linq;

namespace Axis.Dia.Json.Tests.Deserializers
{
    [TestClass]
    public class ValueDeserializer_Tests
    {
        #region Ref
        [TestMethod]
        public void DeserializeRef_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeRef(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeRef("", null!));
        }

        [TestMethod]
        public void DeserializeRef_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeRef("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeRef("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeRef_WithValidRef()
        {
            var @ref = ValueDeserializer.DeserializeRef($"#Ref 0x34", new DeserializerContext());
            Assert.AreEqual(0x34u, @ref.Value);

            @ref = ValueDeserializer.DeserializeRef($"#Ref      0x34", new DeserializerContext());
            Assert.AreEqual(0x34u, @ref.Value);
        }
        #endregion

        #region Blob
        [TestMethod]
        public void DeserializeBlob_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeBlob(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeBlob("", null!));
        }

        [TestMethod]
        public void DeserializeBlob_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeBlob("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeBlob("#Int 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeBlob_WithValidBlob()
        {
            var bytes = new byte[30];
            Random.Shared.NextBytes(bytes);
            var b64 = Convert.ToBase64String(bytes);

            var blob = ValueDeserializer.DeserializeBlob($"#Blob {b64}", new DeserializerContext());
            Assert.IsFalse(blob.IsDefault);
            CollectionAssert.AreEquivalent(bytes, blob.Value);


            blob = ValueDeserializer.DeserializeBlob($"#Blob.null", new DeserializerContext());
            Assert.IsTrue(blob.IsDefault);
            Assert.IsTrue(blob.IsNull);
        }
        #endregion

        #region Bool
        [TestMethod]
        public void DeserializeBool_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeBool(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeBool("", null!));
        }

        [TestMethod]
        public void DeserializeBool_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeBool("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeBool("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeBool_WithValidBool()
        {
            var @bool = ValueDeserializer.DeserializeBool($"#Bool true", new DeserializerContext());
            Assert.IsFalse(@bool.IsDefault);
            Assert.IsTrue(@bool.Value);

            @bool = ValueDeserializer.DeserializeBool($"#Bool      False", new DeserializerContext());
            Assert.IsFalse(@bool.IsDefault);
            Assert.IsFalse(@bool.Value);

            @bool = ValueDeserializer.DeserializeBool($"#Bool.null [@something;]", new DeserializerContext());
            Assert.IsTrue(@bool.IsNull);
        }
        #endregion

        #region Int
        [TestMethod]
        public void DeserializeInt_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeInt(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeInt("", null!));
        }

        [TestMethod]
        public void DeserializeInt_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeInt("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeInt("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeInt_WithValidInt()
        {
            var @int = ValueDeserializer.DeserializeInt($"#Int 34", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(34, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"#Int -221", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(-221, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"#Int      0x34", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(0x34, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"#Int      0xe", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(-2, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"56", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(56, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"0x56", new DeserializerContext());
            Assert.IsFalse(@int.IsDefault);
            Assert.AreEqual(0x56, @int.Value);

            @int = ValueDeserializer.DeserializeInt($"#Int.null [@something;]", new DeserializerContext());
            Assert.IsTrue(@int.IsNull);
        }
        #endregion

        #region Decimal
        [TestMethod]
        public void DeserializeDecimal_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeDecimal(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeDecimal("", null!));
        }

        [TestMethod]
        public void DeserializeDecimal_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeDecimal("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeDecimal("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeDecimal_WithValidDecimal()
        {
            var @decimal = ValueDeserializer.DeserializeDecimal($"#Decimal 34.0", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(34, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"#Decimal -0.221", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(-0.221m, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"#Decimal      3.4e-3", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(3.4e-3m, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"#Decimal      -43e2", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(-43e2m, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"0.001", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(0.001m, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"2.344e-2", new DeserializerContext());
            Assert.IsFalse(@decimal.IsDefault);
            Assert.AreEqual(2.344e-2m, @decimal.Value);

            @decimal = ValueDeserializer.DeserializeDecimal($"#Decimal.null [@something;]", new DeserializerContext());
            Assert.IsTrue(@decimal.IsNull);
        }
        #endregion

        #region Duration
        [TestMethod]
        public void DeserializeDuration_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeDuration(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeDuration("", null!));
        }

        [TestMethod]
        public void DeserializeDuration_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeDuration("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeDuration("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeDuration_WithValidDuration()
        {
            var ts = TimeSpan.FromSeconds(234.4);
            var duration = ValueDeserializer.DeserializeDuration(
                $"#Duration[@key:value;] {ts.ToString(ValueDeserializer.DurationFormat)}",
                new DeserializerContext());
            Assert.IsFalse(duration.IsDefault);
            Assert.AreEqual(ts, duration.Value);

            duration = ValueDeserializer.DeserializeDuration($"#Duration.null", new DeserializerContext());
            Assert.IsTrue(duration.IsNull);
        }
        #endregion

        #region Timestamp
        [TestMethod]
        public void DeserializeTimestamp_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeTimestamp(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeTimestamp("", null!));
        }

        [TestMethod]
        public void DeserializeTimestamp_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeTimestamp("abcd   ", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeTimestamp("#Xyz 56", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeTimestamp_WithValidTimestamp()
        {
            var ts = DateTimeOffset.Now;
            var timestamp = ValueDeserializer.DeserializeTimestamp(
                $"#Timestamp[@key:value;] {ts.ToString(ValueDeserializer.TimestampFormat)}",
                new DeserializerContext());
            Assert.IsFalse(timestamp.IsDefault);
            Assert.AreEqual(ts, timestamp.Value);

            timestamp = ValueDeserializer.DeserializeTimestamp($"#Timestamp.null", new DeserializerContext());
            Assert.IsTrue(timestamp.IsNull);
        }
        #endregion

        #region String
        [TestMethod]
        public void DeserializeString_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeString(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeString("", null!));
        }

        [TestMethod]
        public void DeserializeString_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeString("#String '", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeString("#String 'd", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeString("#String '\\y'", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeString_WithValidString()
        {
            var str = "the quick fox no longer cares";
            var @string = ValueDeserializer.DeserializeString(
                $"#String[@key:value;] '{str}'",
                new DeserializerContext());
            Assert.IsFalse(@string.IsDefault);
            Assert.AreEqual(str, @string.Value);

            @string = ValueDeserializer.DeserializeString($"#String.null", new DeserializerContext());
            Assert.IsTrue(@string.IsNull);

            @string = ValueDeserializer.DeserializeString($"#String ''", new DeserializerContext());
            Assert.AreEqual(string.Empty, @string.Value);

            @string = ValueDeserializer.DeserializeString($"#String 'abc\\t123\\''", new DeserializerContext());
            Assert.AreEqual("abc\t123\'", @string.Value);

            @string = ValueDeserializer.DeserializeString($"abcd", new DeserializerContext());
            Assert.AreEqual("abcd", @string.Value);

            @string = ValueDeserializer.DeserializeString($"##String abcd", new DeserializerContext());
            Assert.AreEqual("#String abcd", @string.Value);
        }
        #endregion

        #region Symbol
        [TestMethod]
        public void DeserializeSymbol_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeSymbol(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeSymbol("", null!));
        }

        [TestMethod]
        public void DeserializeSymbol_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeSymbol("#Symbol '", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeSymbol("#Symbol 'd", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeSymbol("#Symbol '\\y'", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeSymbol("abcd", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeSymbol_WithValidSymbol()
        {
            var str = "the quick fox no longer cares";
            var @symbol = ValueDeserializer.DeserializeSymbol(
                $"#Symbol[@key:value;] '{str}'",
                new DeserializerContext());
            Assert.IsFalse(@symbol.IsDefault);
            Assert.AreEqual(str, @symbol.Value);

            @symbol = ValueDeserializer.DeserializeSymbol($"#Symbol.null", new DeserializerContext());
            Assert.IsTrue(@symbol.IsNull);

            @symbol = ValueDeserializer.DeserializeSymbol($"#Symbol ''", new DeserializerContext());
            Assert.AreEqual(string.Empty, @symbol.Value);

            @symbol = ValueDeserializer.DeserializeSymbol($"#Symbol 'abc\\t123\\''", new DeserializerContext());
            Assert.AreEqual("abc\t123\'", @symbol.Value);
        }
        #endregion

        #region Sequence
        [TestMethod]
        public void DeserializeNullSequence_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeNullSequence(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeNullSequence("", null!));
        }

        [TestMethod]
        public void DeserializeNullSequence_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullSequence("#Sequence", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullSequence("#Sequence[@abcd;]", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullSequence("#Sequence [@abcd']", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullSequence("[1, 2, 3]", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeNullSequence_WithValidSequence()
        {
            var sequence = ValueDeserializer.DeserializeNullSequence("#Sequence.null", new DeserializerContext());
            Assert.IsTrue(sequence.IsDefault);
            Assert.AreEqual(0, sequence.Count);
            Assert.AreEqual(0, sequence.Attributes.Count);

            sequence = ValueDeserializer.DeserializeNullSequence($"#Sequence.null[@abcd;]", new DeserializerContext());
            Assert.IsTrue(sequence.IsNull);
            Assert.AreEqual(1, sequence.Attributes.Count);

            sequence = ValueDeserializer.DeserializeNullSequence($"#Sequence.null [@abcd:123;]", new DeserializerContext());
            Assert.IsTrue(sequence.IsNull);
            Assert.AreEqual(1, sequence.Attributes.Count);
        }

        [TestMethod]
        public void DeserializeSequence_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeSequence(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeSequence([], null!));
        }

        [TestMethod]
        public void DeserializeSequence_WithValidSequence()
        {
            var sequence = ValueDeserializer.DeserializeSequence([], new DeserializerContext());
            Assert.IsFalse(sequence.IsDefault);
            Assert.AreEqual(0, sequence.Count);

            sequence = ValueDeserializer.DeserializeSequence(
                context: new DeserializerContext(),
                array: [new JObject()]);
            Assert.IsFalse(sequence.IsDefault);
            Assert.AreEqual(1, sequence.Count);

            sequence = ValueDeserializer.DeserializeSequence(
                context: new DeserializerContext(),
                array: [new JObject
                {
                    [ValueDeserializer.ValueRefMetadataProperty] = "0x4"
                }]);
            Assert.IsFalse(sequence.IsDefault);
            Assert.AreEqual(0, sequence.Count);

            sequence = ValueDeserializer.DeserializeSequence(
                context: new DeserializerContext(),
                array: [new JObject
                {
                    [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]"
                }]);
            Assert.IsFalse(sequence.IsDefault);
            Assert.AreEqual(0, sequence.Count);
            Assert.AreEqual(1, sequence.Attributes.Count);

            var cxt = new DeserializerContext();
            sequence = ValueDeserializer.DeserializeSequence(
                context: cxt,
                array: [
                    new JObject
                    {
                        [ValueDeserializer.ValueRefMetadataProperty] = "0x4",
                        [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]"
                    },
                    "#Int 544",
                    "#Ref 0x0"
                ]);
            Assert.IsFalse(sequence.IsDefault);
            Assert.AreEqual(2, sequence.Count);
            Assert.AreEqual(1, sequence.Attributes.Count);
            Assert.AreEqual(1, cxt.RefInstances.Length);
            Assert.AreEqual(1, cxt.RefInstances[0].Key);
            Assert.AreEqual(sequence, cxt.RefInstances[0].Parent);
        }
        #endregion

        #region Record
        [TestMethod]
        public void DeserializeNullRecord_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeNullRecord(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeNullRecord("", null!));
        }

        [TestMethod]
        public void DeserializeNullRecord_WithInvalidValues()
        {
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullRecord("#Record", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullRecord("#Record[@abcd;]", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullRecord("#Record [@abcd']", new DeserializerContext()));
            Assert.Throws<InvalidOperationException>(
                () => ValueDeserializer.DeserializeNullRecord("[1, 2, 3]", new DeserializerContext()));
        }

        [TestMethod]
        public void DeserializeNullRecord_WithValidRecord()
        {
            var record = ValueDeserializer.DeserializeNullRecord("#Record.null", new DeserializerContext());
            Assert.IsTrue(record.IsDefault);
            Assert.AreEqual(0, record.Count);
            Assert.AreEqual(0, record.Attributes.Count);

            record = ValueDeserializer.DeserializeNullRecord($"#Record.null[@abcd;]", new DeserializerContext());
            Assert.IsTrue(record.IsNull);
            Assert.AreEqual(1, record.Attributes.Count);

            record = ValueDeserializer.DeserializeNullRecord($"#Record.null [@abcd:123;]", new DeserializerContext());
            Assert.IsTrue(record.IsNull);
            Assert.AreEqual(1, record.Attributes.Count);
        }

        [TestMethod]
        public void DeserializeRecord_WithNullValues()
        {
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeRecord(null!, new DeserializerContext()));
            Assert.Throws<ArgumentNullException>(
                () => ValueDeserializer.DeserializeRecord([], null!));
        }

        [TestMethod]
        public void DeserializeRecord_WithValidRecord()
        {
            var record = ValueDeserializer.DeserializeRecord([], new DeserializerContext());
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(0, record.Count);

            record = ValueDeserializer.DeserializeRecord(
                context: new DeserializerContext(),
                jobj: new JObject
                {
                    ["regular"] = new JObject
                    {
                        [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]"
                    }
                });
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(1, record.Count);

            record = ValueDeserializer.DeserializeRecord(
                context: new DeserializerContext(),
                jobj: new JObject
                {
                    [ValueDeserializer.MetadataInstanceProperty] = new JObject
                    {
                        [ValueDeserializer.ValueRefMetadataProperty] = "0x4"
                    }
                });
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(0, record.Count);

            record = ValueDeserializer.DeserializeRecord(
                context: new DeserializerContext(),
                jobj: new JObject
                {
                    [ValueDeserializer.MetadataInstanceProperty] = new JObject
                    {
                        [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]"
                    }
                });
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(0, record.Count);
            Assert.AreEqual(1, record.Attributes.Count);

            record = ValueDeserializer.DeserializeRecord(
                context: new DeserializerContext(),
                jobj: new JObject
                {
                    [ValueDeserializer.MetadataInstanceProperty] = new JObject
                    {
                        [ValueDeserializer.ValueRefMetadataProperty] = "0x4",
                        [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]"
                    }
                });
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(0, record.Count);
            Assert.AreEqual(1, record.Attributes.Count);

            var cxt = new DeserializerContext();
            record = ValueDeserializer.DeserializeRecord(
                context: cxt,
                jobj: new JObject
                {
                    ["regular"] = 456,
                    ["kebab"] = "#Ref 0x1",
                    [ValueDeserializer.MetadataInstanceProperty] = new JObject
                    {
                        [ValueDeserializer.ValueRefMetadataProperty] = "0x4",
                        [ValueDeserializer.ValueAttributeMetadataProperty] = "[@abcd;]",
                        ["$regular"] = "[@xyz;]",
                    }
                });
            Assert.IsFalse(record.IsDefault);
            Assert.AreEqual(2, record.Count);
            Assert.AreEqual(1, record.Attributes.Count);
            Assert.AreEqual(1, cxt.RefInstances.Length);
            Assert.AreEqual("kebab", cxt.RefInstances[0].Key);
            Assert.AreEqual(record, cxt.RefInstances[0].Parent);
        }
        #endregion
    }
}
