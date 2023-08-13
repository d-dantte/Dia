using Axis.Luna.Extensions;
using Axis.Pulsar.Grammar.CST;
using Axis.Pulsar.Grammar.Language.Rules;

namespace Axis.Dia.Convert.Text.Pulsar.Validators
{
    public class BlobProductionValidator : IProductionValidator
    {
        private const string Blob = "blob-text-value";

        public ProductionValidationResult ValidateCSTNode(ProductionRule rule, CSTNode node)
        {
            if (!Blob.Equals(rule.SymbolName))
                return new ProductionValidationResult.Error(
                "Symbol name mismatch: "
                    + $"expected: {Blob}, "
                    + $"rule: {rule.SymbolName}, "
                    + $"node: {node.SymbolName}");

            var tokens = node.TokenValue();
            return tokens
                .Replace("<", "")
                .Replace(">", "")
                .Replace(" ", "")
                .Replace("\t", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\v", "")
                .Replace("\f", "")
                .ApplyTo(ValidateB64String);
        }

        private ProductionValidationResult ValidateB64String(string b64)
        {
            try
            {
                var blob = System.Convert.FromBase64String(b64);
                return new ProductionValidationResult.Success();
            }
            catch (Exception ex)
            {
                return new ProductionValidationResult.Error(ex.Message);
            }
        }
    }
}
