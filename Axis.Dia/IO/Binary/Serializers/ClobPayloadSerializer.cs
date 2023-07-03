﻿using Axis.Dia.Contracts;
using Axis.Dia.IO.Binary.Metadata;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using System.Numerics;
using System.Text;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal class ClobPayloadSerializer :
        IPayloadSerializer<ClobValue>,
        IPayloadProvider<ClobValue>
    {
        // prohibits instantiation
        private ClobPayloadSerializer() { }

        public static ValuePayload<ClobValue> CreatePayload(ClobValue value)
        {
            TypeMetadata.MetadataFlags metadataFlags =
                (!value.Annotations.IsEmpty() ? TypeMetadata.MetadataFlags.Annotated : TypeMetadata.MetadataFlags.None)
                | (value.IsNull ? TypeMetadata.MetadataFlags.Null : TypeMetadata.MetadataFlags.None)
                | (value.Value?.Length > 0 ? TypeMetadata.MetadataFlags.Overflow : TypeMetadata.MetadataFlags.None);

            BigInteger? charCount = value.Value?.Length switch
            {
                null or 0 => null,
                int v => new BigInteger(v)
            };

            var typeMetadata = TypeMetadata.Of(
                DiaType.Clob,
                metadataFlags,
                charCount);

            return new ValuePayload<ClobValue>(value, typeMetadata);
        }

        public static IResult<ClobValue> Deserialize(Stream stream, TypeMetadata typeMetadata, BinarySerializerContext context)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(context);

            if (!DiaType.Clob.Equals(typeMetadata.Type))
                throw new ArgumentException($"Invalid TypeMetadata.Type: '{typeMetadata.Type}', expected: '{DiaType.Clob}'");

            return Result
                .Of(typeMetadata)

                // read annotations and determine how many bytes/chars to read for the symbol
                .Map(tmeta => (
                    tmeta.IsNull,
                    CharCount: tmeta.CustomMetadataCount > 0
                        ? (int)tmeta.CustomMetadata.ToBigInteger()
                        : 0,
                    Annotations: tmeta.IsAnnotated
                        ? AnnotationSerializer.Deserialize(stream).Resolve()
                        : Array.Empty<Annotation>()))

                // read and construct the symbol
                .Map(tuple => (
                    tuple.Annotations,
                    Bytes: tuple.IsNull ? null :
                        tuple.CharCount == 0 ? "" :
                        stream
                            .ReadExactBytesResult(tuple.CharCount * 2)
                            .Map(Encoding.Unicode.GetString)
                            .Resolve()))

                // construct the SymbolValue
                .Map(tuple => ClobValue.Of(tuple.Bytes, tuple.Annotations));
        }

        public static IResult<byte[]> Serialize(ClobValue value, BinarySerializerContext context)
        {
            try
            {
                var typeMetadataResult = Result.Of(CreatePayload(value)).Map(payload => payload.TypeMetadata);
                var annotationResult = AnnotationSerializer.Serialize(value.Annotations);
                var clobDataResult = (value.Value?.Length ?? 0) == 0
                    ? Result.Of(Array.Empty<byte>)
                    : Result.Of(() => Encoding.Unicode.GetBytes(value.Value!));

                return typeMetadataResult

                    // first the type-metadata bytes
                    .Map(tmeta => tmeta.Metadata.ToArray())

                    // next the annotation bytes
                    .Combine(annotationResult, (bytes, annotationBytes) => bytes.Concat(annotationBytes))

                    // finally, the blob bytes
                    .Combine(clobDataResult, (bytes, blobBytes) => bytes.Concat(blobBytes))
                    .Map(bytes => bytes.ToArray());
            }
            catch (Exception ex)
            {
                return Result.Of<byte[]>(ex);
            }
        }
    }
}