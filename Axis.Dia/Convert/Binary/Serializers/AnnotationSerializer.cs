using Axis.Dia.Types;
using Axis.Dia.Utils;
using Axis.Luna.Extensions;
using System.Numerics;
using System.Text;

namespace Axis.Dia.Convert.Binary.Serializers
{
    internal static class AnnotationSerializer
    {
        public static IResult<Annotation[]> Deserialize(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            return stream
                .ReadVarBytesResult()
                .Map(varbytes => new BigInteger(varbytes.ToByteArray()!))
                .Bind(annotationCount => DeserializeAnnotations(annotationCount, stream));
        }

        public static IResult<byte[]> Serialize(params Annotation[] annotations)
        {
            ArgumentNullException.ThrowIfNull(annotations);

            try
            {
                if (annotations.Length == 0)
                    return Result.Of(Array.Empty<byte>);

                var annotationCountVarBytes = new BigInteger(annotations.Length)
                    .ToBitSequence()
                    .ApplyTo(VarBytes.Of);

                return annotations
                    .Select(annotation => SerializeAnnotation(annotation))
                    .Fold()
                    .Map(annotationBytes => annotationCountVarBytes
                        .Concat(annotationBytes.SelectMany())
                        .ToArray());
            }
            catch(Exception e)
            {
                return Result.Of<byte[]>(e);
            }
        }

        private static IResult<Annotation[]> DeserializeAnnotations(
            BigInteger count,
            Stream stream)
        {
            return count
                .Repeat(_ => DeserializeAnnotation(stream))
                .Fold()
                .Map(annotations => annotations.ToArray());
        }

        private static IResult<Annotation> DeserializeAnnotation(Stream stream)
        {
            return stream
                .ReadVarBytesResult()
                .Map(varbytes => new BigInteger(varbytes.ToByteArray()!))
                .Map(bigInt => (int)bigInt)
                .Bind(charCount => stream.ReadExactBytesResult(charCount*2))
                .Map(Encoding.Unicode.GetString)
                .Map(Annotation.Of);
        }

        private static IResult<byte[]> SerializeAnnotation(Annotation annotation)
        {
            if (annotation.IsDefault)
                return Result.Of<byte[]>(new ArgumentException($"Invalid annotation: {annotation}"));

            return new BigInteger(annotation.Text.Length)
                .ToBitSequence()
                .ApplyTo(VarBytes.Of)
                .Concat(Encoding.Unicode.GetBytes(annotation.Text))
                .ToArray()
                .ApplyTo(Result.Of);
        }
    }
}
