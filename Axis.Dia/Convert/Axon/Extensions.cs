﻿using Axis.Dia.Convert.Axon.Parsers;
using Axis.Luna.Common.Results;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Axon
{
    internal static class Extensions
    {
        internal static (CSTNode? AddressIndexNode, CSTNode? AnnotationNode, CSTNode ValueNode) DeconstructValueNode(
            this CSTNode valueRoot)
        {
            ArgumentNullException.ThrowIfNull(valueRoot);

            var addressIndexNode = valueRoot
                .FindNodes("value-address/address-index")
                .FirstOrDefault();

            var annotationNode = valueRoot
                .FindNodes(AnnotationParser.GrammarSymbol)
                .FirstOrDefault();

            var valueNode = valueRoot.LastNode();

            return (addressIndexNode, annotationNode, valueNode);
        }

        internal static string ReverseString(this string s)
        {
            var array = new char[s.Length];
            int forward = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                array[forward++] = s[i];
            }
            return new string(array);
        }
    }
}
