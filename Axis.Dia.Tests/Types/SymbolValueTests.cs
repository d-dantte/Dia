using Axis.Dia.Contracts;
using Axis.Dia.Types;

namespace Axis.Dia.Tests.Types
{
    [TestClass]
    public class SymbolValueTests
    {
        [TestMethod]
        public void Creation_Tests()
        {
            #region null symbol
            var symbol = new SymbolValue();

            Assert.AreEqual(null, symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);


            symbol = new SymbolValue(Annotation.Of("first", "second"));

            Assert.AreEqual(null, symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(2, symbol.Annotations.Length);


            symbol = new SymbolValue(null, "first", "second");

            Assert.AreEqual(null, symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(2, symbol.Annotations.Length);
            #endregion

            #region non-null symbol
            symbol = new SymbolValue("the symbol");

            Assert.AreEqual("the symbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);


            symbol = new SymbolValue("TheSymbol", Annotation.Of("first", "second"));

            Assert.AreEqual("TheSymbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(2, symbol.Annotations.Length);


            symbol = new SymbolValue("The.Symbol", "first", "second");

            Assert.AreEqual("The.Symbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(2, symbol.Annotations.Length);
            #endregion

            #region implicits
            symbol = "the symbol";

            Assert.AreEqual("the symbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);


            symbol = "the.symbol".ToArray();

            Assert.AreEqual("the.symbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);


            symbol = new Span<char>("the-symbol".ToArray());

            Assert.AreEqual("the-symbol", symbol.Value);
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);
            #endregion
        }

        [TestMethod]
        public void Type_Tests()
        {
            var symbol = default(SymbolValue);
            Assert.AreEqual(DiaType.Symbol, symbol.Type);
        }

        [TestMethod]
        public void Value_Tests()
        {
            var symbol = SymbolValue.Of(null);
            Assert.IsNull(symbol.Value);

            symbol = "something";
            Assert.IsNotNull(symbol.Value);
        }

        [TestMethod]
        public void IsNull_Tests()
        {
            var symbol = SymbolValue.Of(null);
            Assert.IsTrue(symbol.IsNull);

            symbol = SymbolValue.Of(null, Annotation.Of("bleh"));
            Assert.IsTrue(symbol.IsNull);

            symbol = "something";
            Assert.IsFalse(symbol.IsNull);
        }

        [TestMethod]
        public void Annotation_Tests()
        {
            var symbol = new SymbolValue(null, Annotation.Of("bleh", "other"));
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(2, symbol.Annotations.Length);

            symbol = default;
            Assert.IsNotNull(symbol.Annotations);
            Assert.AreEqual(0, symbol.Annotations.Length);

            Assert.ThrowsException<ArgumentNullException>(() => new SymbolValue(null, null as Annotation[]));
            Assert.ThrowsException<ArgumentException>(() => new SymbolValue(null, "stuff", default));
        }

        [TestMethod]
        public void DeepCopy_Tests()
        {
            SymbolValue symbol = default;
            var copied = symbol.DeepCopy();

            Assert.AreEqual(symbol, copied);
            Assert.AreEqual(symbol.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(symbol.Value, copied.Value);

            symbol = SymbolValue.Of("stuff", Annotation.Of("bleh"));
            copied = symbol.DeepCopy();

            Assert.AreEqual(symbol, copied);
            Assert.AreEqual(symbol.Annotations.Length, copied.Annotations.Length);
            Assert.AreEqual(symbol.Value, copied.Value);
        }

        [TestMethod]
        public void Null_Tests()
        {
            var symbol = SymbolValue.Null();
            Assert.AreEqual(default, symbol);

            symbol = SymbolValue.Null(Annotation.Of("stuff"));
            Assert.IsFalse(default(SymbolValue).ValueEquals(symbol));
            Assert.IsTrue(default(SymbolValue).ValueEquals(symbol));
            Assert.AreEqual(default(SymbolValue).Value, symbol.Value);
            Assert.AreNotEqual(default(SymbolValue).Annotations.Length, symbol.Annotations.Length);
        }

        [TestMethod]
        public void ValueEquals_Tests()
        {
            SymbolValue first = "something";
            SymbolValue second = "other something";
            SymbolValue third = SymbolValue.Of("something", Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsTrue(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void EqualsTest()
        {
            SymbolValue first = "something";
            SymbolValue second = "other something";
            SymbolValue third = SymbolValue.Of("something", Annotation.Of("annotated"));

            Assert.IsTrue(first.ValueEquals(first));
            Assert.IsFalse(first.ValueEquals(third));
            Assert.IsFalse(first.ValueEquals(second));
        }

        [TestMethod]
        public void IsIdentifier_Tests()
        {
            var symbol = SymbolValue.Of("abcdef");
            Assert.IsTrue(symbol.IsIdentifier);

            symbol = "abcd.efgh";
            Assert.IsTrue(symbol.IsIdentifier);

            symbol = "_abc_d.efg_h_";
            Assert.IsTrue(symbol.IsIdentifier);

            symbol = "ab.cd-efg-h";
            Assert.IsTrue(symbol.IsIdentifier);

            symbol = "_";
            Assert.IsTrue(symbol.IsIdentifier);

            symbol = "2abc";
            Assert.IsFalse(symbol.IsIdentifier);

            symbol = "abc xyz";
            Assert.IsFalse(symbol.IsIdentifier);

            symbol = ".ab.cd-efg-h";
            Assert.IsFalse(symbol.IsIdentifier);

            symbol = "-ab.cd-efg-h";
            Assert.IsFalse(symbol.IsIdentifier);
        }
    }
}
