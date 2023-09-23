using Axis.Dia.Contracts;
using Axis.Dia.Exceptions;
using Axis.Dia.Types;
using Axis.Luna.Extensions;

namespace Axis.Dia.Utils
{
    public static class ReferenceUtil
    {

        /// <summary>
        /// Links all references found in the object graph starting at the given <paramref name="rootValue"/>
        /// </summary>
        /// <param name="rootValue">The value from which references should be found</param>
        /// <exception cref="ReferenceLinkageException"></exception>
        public static IDiaValue LinkReferences(this IDiaValue rootValue, out IDiaReference[] linkedReferences)
        {
            var refMap = new Dictionary<Guid, RefInfo>();
            BuildReferenceMap(rootValue, refMap);
            LinkChildReferences(refMap);

            linkedReferences = refMap.Values
                .Where(info => info.AddressProvider is not null)
                .SelectMany(info => info.References)
                .ToArray();

            var unlinkedReferences = refMap.Values
                .Where(info => info.AddressProvider is null)
                .SelectMany(info => info.References)
                .ToArray();

            if (unlinkedReferences.Length > 0)
                throw new ReferenceLinkageException(rootValue, unlinkedReferences);

            return rootValue;
        }


        /// <summary>
        /// Ensures that all <see cref="ReferenceValue"/> instances in the document are linkable, then goes ahead to link them
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unlinkableReferences"></param>
        /// <returns></returns>
        public static bool TryLinkReferences(this
            IDiaValue value,
            out IDiaReference[] linkedReferences,
            out IDiaReference[] unlinkedReferences)
        {
            try
            {
                _ = LinkReferences(value, out linkedReferences);
                unlinkedReferences = Array.Empty<IDiaReference>();
                return true;
            }
            catch(ReferenceLinkageException rle)
            {
                linkedReferences = Array.Empty<IDiaReference>();
                unlinkedReferences = rle.References.ToArray();
                return false;
            }
            catch
            {
                linkedReferences = Array.Empty<IDiaReference>();
                unlinkedReferences = Array.Empty<IDiaReference>();
                return false;
            }
        }

        private static void BuildReferenceMap(
            IDiaValue value,
            Dictionary<Guid, RefInfo> referenceMap)
        {
            if (value is IDiaReference reference)
            {
                var info = referenceMap.GetOrAdd(reference.ValueAddress, add => new RefInfo());
                info.References.Add(reference);
            }
            else if(value is IDiaAddressProvider addressable)
            {
                var info = referenceMap.GetOrAdd(addressable.Address, add => new RefInfo());

                if (info.AddressProvider is not null)
                    throw new InvalidOperationException(
                        $"Duplicate address found for values: '{addressable}', and '{info.AddressProvider}'");

                info.AddressProvider = addressable;

                if (addressable is ListValue list && !list.IsNull)
                    list.Value!.ForAll(item => BuildReferenceMap(item, referenceMap));

                else if (addressable is RecordValue record && !record.IsNull)
                    record.Value!.ForAll(item => BuildReferenceMap(item.Value, referenceMap));
            }
        }

        private static void LinkChildReferences(Dictionary<Guid, RefInfo> refMap)
        {
            refMap
                .Where(kvp => kvp.Value.AddressProvider is not null)
                .Where(kvp => kvp.Value.References.Count > 0)
                .ForAll(kvp => kvp.Value.References.ForEach(@ref =>
                    @ref.ReboxAs(ReferenceValue.Of(
                        kvp.Key,
                        kvp.Value.AddressProvider!,
                        @ref.Annotations))));
        }

        internal record RefInfo
        {
            public IDiaAddressProvider? AddressProvider { get; set; }

            public List<IDiaReference> References { get; } = new();
        }
    }
}
