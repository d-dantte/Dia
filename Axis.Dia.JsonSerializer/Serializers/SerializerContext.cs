namespace Axis.Dia.Json.Serializers
{
    internal class SerializerContext
    {
        private readonly ReferenceMap referenceMap = new();

        public ReferenceMap ReferenceMap => referenceMap;
    }
}
