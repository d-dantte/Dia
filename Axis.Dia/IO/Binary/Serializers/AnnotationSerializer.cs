using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.IO.Binary.Serializers
{
    internal static class AnnotationSerializer
    {
        public static IResult<Annotation[]> Deserialize(Stream stream, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }

        public static IResult<byte[]> Serialize(Annotation[] value, BinarySerializerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
