﻿using Axis.Pulsar.Grammar.Language.Rules.CustomTerminals;
using Axis.Pulsar.Grammar.Language;
using Axis.Pulsar.Grammar.Recognizers;
using Axis.Dia.IO.Text.Pulsar.Recognizers;

namespace Axis.Dia.IO.Text.Pulsar.Rules
{
    public class SymbolIdentifierRule : ICustomTerminal
    {
        public static string[] KeyWords => _keyWords.ToArray();

        private static readonly string[] _keyWords = new string[]
        {
            "null", "true", "false", "nan"
        };

        public string SymbolName => "SymbolIdentifier";

        public IRecognizer ToRecognizer(Grammar grammar) => new SymbolIdentifierRecognizer(grammar, this);
    }
}
