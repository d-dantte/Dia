using Axis.Dia.Convert.Type.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Axis.Dia.Tests.Convert.Type.Converters
{
    [TestClass]
    public class EnumConverterTests
    {
        private static readonly EnumConverter converter = new EnumConverter();

        //[TestMethod]
        //public void CanConvertToDia_Tests()
        //{
        //    var canConvert = converter.CanConvert(typeof(SampleEnum), SampleEnum.One);
        //    Assert.IsTrue(canConvert);

        //    Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(null, SampleEnum.One));
        //}

        //[TestMethod]
        //public void CanConvertToClr_Tests()
        //{
        //    var canConvert = converter.CanConvert(typeof(SampleEnum), IonTextSymbol.Null());
        //    Assert.IsTrue(canConvert);

        //    Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(null, IonTextSymbol.Null()));
        //    Assert.ThrowsException<ArgumentNullException>(() => converter.CanConvert(typeof(IonTypes), null));
        //}

        //[TestMethod]
        //public void ToClr_Tests()
        //{
        //    var options = new ConversionContext(ConversionOptionsBuilder.NewBuilder().Build());
        //    var enumType = typeof(IonTypes);
        //    var @string = new IonString("Sexp");
        //    var identifier = new IonTextSymbol("Sexp");

        //    var value = converter.ToClr(enumType, @string, options);
        //    Assert.AreEqual(IonTypes.Sexp, value);

        //    value = converter.ToClr(enumType, identifier, options);
        //    Assert.AreEqual(IonTypes.Sexp, value);

        //    Assert.ThrowsException<ArgumentNullException>(() => converter.ToClr(null, @string, options));

        //    var invalidEnum = new IonString("Bleh");
        //    Assert.ThrowsException<ArgumentException>(() => converter.ToClr(enumType, invalidEnum, options));

        //    var nullEnum = IIonValue.NullOf(IonTypes.TextSymbol);
        //    Assert.ThrowsException<ArgumentException>(() => converter.ToClr(enumType, nullEnum, options));
        //}

        //[TestMethod]
        //public void ToIon_Tests()
        //{
        //    var options = new ConversionContext(ConversionOptionsBuilder.NewBuilder().Build());
        //    var enumType = typeof(IonTypes);
        //    var result = converter.ToIon(enumType, IonTypes.Blob, options);
        //    var ion = (IonTextSymbol)result;

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(IonTypes.Blob.ToString(), ion.Value);

        //    result = converter.ToIon(enumType, null, options);
        //    ion = (IonTextSymbol)result;

        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(ion.IsNull);
        //}

        //public enum SampleEnum
        //{
        //    One,
        //    Two,
        //    Three,
        //    Four,
        //    Five
        //}
    }
}
