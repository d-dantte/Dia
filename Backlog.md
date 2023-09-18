#### 2023/09/08
1. Both `ListValue` and `RecordValue` should be changed to `struct` types. The reason behind this that the structural value of the types are preserved
   when they exist as structs, and are copied across method boundaries. Most important thing to note is that they are both immutable - only things
   changing are the addition or removal of values from the lists they encapsulate, which, again, preserves the integrity of the value.


#### 2023/09/13
1. Include support for `ReferenceValue` in the the `Binary` serializer. Simply keep a list of serialized values, and store a ref with the index into
   that list every time a ref of an already serialized value is encountered - essentially the same strategy used for the SymbolHashList in the `ion`
   serializer implementation, but for all values.
2. Include support for `ReferenceValue` in the `Text` serializer. Simply use a new delimiter for the reference, and store the address guid. This also
   means that all values may have their guids serialized. To optimize the process, only values that are referenced will have their guids serialized,
   so the reference can refer to the guid.
2. Implement the `JSON` conversion API.
