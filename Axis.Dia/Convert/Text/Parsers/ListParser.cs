﻿using Axis.Dia.Types;
using Axis.Luna.Common.Results;
using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;

namespace Axis.Dia.Convert.Text.Parsers
{
    public class ListParser : IValueSerializer<ListValue>
    {
        #region Symbols
        public const string SymbolNameDiaList = "dia-list";
        public const string SymbolNameNullList = "null-list";
        public const string SymbolNameListValue = "list-value";
        public const string SymbolNameDiaValue = "dia-value";
        #endregion


        private ListParser() { }

        public static string GrammarSymbol => SymbolNameDiaList;

        public static IResult<ListValue> Parse(CSTNode symbolNode, TextSerializerContext? context = null)
        {
            ArgumentNullException.ThrowIfNull(symbolNode);

            if (!GrammarSymbol.Equals(symbolNode.SymbolName))
                throw new ArgumentException(
                    $"Invalid symbol name. Expected '{GrammarSymbol}', "
                    + $"but found '{symbolNode.SymbolName}'");

            try
            {
                context ??= new TextSerializerContext();

                var (AnnotationNode, ValueNode) = symbolNode.DeconstructValue();
                var annotationResult = AnnotationNode is null
                    ? Result.Of(Array.Empty<Annotation>())
                    : AnnotationParser.Parse(AnnotationNode, context);

                return ValueNode.SymbolName switch
                {
                    SymbolNameNullList => annotationResult.Map(ListValue.Null),
                    SymbolNameListValue => ValueNode
                        .FindNodes(SymbolNameDiaValue)
                        .Select(node => TextSerializer.ParseValue(node, context.IndentContext()))
                        .Fold()
                        .Combine(
                            annotationResult,
                            (items, annotations) => ListValue.Of(annotations, items.ToArray())),

                    _ => Result.Of<ListValue>(new ArgumentException(
                        $"Invalid symbol: '{ValueNode.SymbolName}'. "
                        + $"Expected '{SymbolNameNullList}', or '{SymbolNameListValue}'"))
                };
            }
            catch (Exception e)
            {
                return Result.Of<ListValue>(e);
            }
        }


        public static IResult<string> Serialize(ListValue value, TextSerializerContext? context = null)
        {
            context ??= new TextSerializerContext();

            var annotationText = AnnotationParser.Serialize(value.Annotations, context);
            var indentedContext = context.IndentContext();
            var indentationText = indentedContext.Indentation();

            (var ldelimiter, var valueSeparator, var rdelimiter) = context.Options.Lists.UseMultipleLines switch
            {
                false => ("[", ", ", "]"),
                true => (
                    $"[{Environment.NewLine}{indentationText}",
                    $",{Environment.NewLine}{indentationText}",
                    $"{Environment.NewLine}{context.Indentation()}]")
            };

            var valueText = value.IsNull switch
            {
                true => Result.Of("null.list"),
                false => value.Value!
                    .Select(item => TextSerializer.SerializeValue(item, context.IndentContext()))
                    .Fold()
                    .Map(items => items
                        .JoinUsing(valueSeparator)
                        .WrapIn(ldelimiter, rdelimiter))
            };

            return annotationText!.Combine(valueText, (ann, value) => $"{ann}{value}");
        }
    }
}
