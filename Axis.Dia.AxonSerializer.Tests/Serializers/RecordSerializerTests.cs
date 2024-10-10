using Axis.Dia.Axon;
using Axis.Dia.Axon.Serializers;
using Axis.Dia.Core.Types;

namespace Axis.Dia.Axon.Tests.Serializers
{
    [TestClass]
    public class RecordSerializerTests
    {
        [TestMethod]
        public void Serialize_Tests()
        {
            Assert.ThrowsException<ArgumentException>(
                () => RecordSerializer.Serialize(default, default));

            #region Singleline
            var options = Options
                .Builder()
                .Build();
            var context = SerializerContext.Of(options);

            var text = RecordSerializer.Serialize(default, context);
            Assert.AreEqual("null.record", text);

            text = RecordSerializer.Serialize(default, context);
            Assert.AreEqual("null.record", text);

            var record = Record.Empty();
            text = RecordSerializer.Serialize(record, context);
            Assert.AreEqual("{}", text);

            record = new Record
            {
                ["prop.1"] = 45
            };

            var record2 = new Record([], "abcd")
            {
                ["prop.1"] = 45
            };
            text = RecordSerializer.Serialize(record, context);
            Assert.AreEqual("#1; {prop.1: 45}", text);
            text = RecordSerializer.Serialize(record, context);
            Assert.AreEqual("'Ref:Record 0x1'", text);
            text = RecordSerializer.Serialize(record2, context);
            Assert.AreEqual("#2; @abcd;{prop.1: 45}", text);
            text = RecordSerializer.Serialize(record2, context);
            Assert.AreEqual("'Ref:Record 0x2'", text);

            var record3 = record;
            text = RecordSerializer.Serialize(record3, context);
            Assert.AreEqual("'Ref:Record 0x1'", text);
            #endregion

            #region Multiline
            options = Options
                .Builder()
                .WithIndentationStyle(Options.IndentationStyle.Spaces)
                .WithRecordAlwaysQuotePropertyName(true)
                .WithRecordUseMultiline(true)
                .Build();

            context = SerializerContext.Of(options);
            record2 = new Record([], "abcd")
            {
                ["prop.1\""] = 45,
                [Record.PropertyName.Of("prop 2", "flag")] = Record.Empty()
            };
            text = RecordSerializer.Serialize(record2, context);
            Assert.AreEqual(@"#0; @abcd;{
    ""prop.1\"""": 45,
    @flag;""prop 2"": {}
}", text);
            #endregion
        }

        [TestMethod]
        public void IsEscapable_Tests()
        {
            Assert.IsTrue(RecordSerializer.IsEscapable('\\'));
            Assert.IsTrue(RecordSerializer.IsEscapable('"'));
            Assert.IsTrue(RecordSerializer.IsEscapable('\n'));
            Assert.IsFalse(RecordSerializer.IsEscapable('a'));
        }
    }
}
