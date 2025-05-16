namespace Axis.Dia.Core.Attributes
{
    /// <summary>
    /// Describes well known types that can be used as attribtues todecorate <see cref="Types.String"/> values.
    /// <para/>
    /// Recommended usage: 
    /// <code>
    /// Core.Types.Attributes.Of(nameof(WellKnownTypes), WellKnownType.Guid)
    /// or
    /// WellKnownType.Guid.ToAttribute()
    /// </code>
    /// </summary>
    public enum WellKnownTypes
    {
        Guid,
        Uri,
        Urn,
        Email,
        Phone,

        #region Geolocation
        GeoISO6709,
        GeoURI, // RFC 5870
        GeoWKT
        #endregion
    }

    public static class WellKnownTypesExtensions
    {
        public static Types.Attribute ToAttribute(this WellKnownTypes type)
        {
            // Note that we do not filter for unknown types. So the following is attainable: @WellKnowntypes:23;
            return Types.Attribute.Of(
                nameof(WellKnownTypes),
                type.ToString());
        }
    }
}
