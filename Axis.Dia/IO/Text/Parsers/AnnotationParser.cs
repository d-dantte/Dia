using System;
using Axis.Dia.Contracts;
using Axis.Dia.Types;
using Axis.Luna.Common.Results;

namespace Axis.Dia.IO.Text.Parsers
{
	public static class AnnotationParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="annotations"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<string> Serialize(
            Annotation[] annotations,
            TextSerializerContext? context = null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IResult<Annotation[]> Parse(
            string text,
            TextSerializerContext? context = null)
        {
        }
    }
}

