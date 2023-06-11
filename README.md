# Dia
> Data representation specification and implementation, supporting both textual (human readable) and binary formats.


## Contents
1. [Introduction](#Introduction)
2. [Specification](#Specification)
3. [Attribute](#Attribute)
4. [Bool](#Bool)
5. [Int](#Int)
6. [Decimal](#Decimal)
7. [Instant](#Instant)
8. [String](#String)
9. [Symbol](#Symbol)
10. [Clob](#Clob)
11. [List](#List)
12. [Record](#Record)
13. [Appendix](#Appendix) (things like key-words, var-bytes, etc will appear here)


## Introduction
Dia is yet another data representation format, born from the need for more feature-sets in json; as such, it is a superset of json.

## Specification
Dia recognizes 9 data types. The concept of types in `Dia` allow for the absence of values: in this case, a null is used. `Dia` types are:
0. Attribute
1. Bool
2. Int
3. Decimal
4. Instant
5. String
6. Symbol
7. Clob
8. List
9. Record

Every dia value represents data of the corresponding type, or the absence of data. All values may also have an optional attribute list attached to them.

As already stated, Dia supports Textual and Binary representation of a specific arrangement of the above types. The following sections will
discuss the details of each of the types, as well as their representation in the 2 formats. However, before proceeding, a general overview of
the binary format is necessary, as there are shared concepts among the types that need to be established first.

#### _1. Data Packet_
At the root of the Dia data model is a data packet. Strictly speaking, it is a list of 0 or more Dia values, where each Dia value can be of
any of the supported types. It is in this form that data is transmitted or stored, or utilized.

#### _2. Binary Representation_
In binary representation, every value exists as a self-contained entity, independently from other values - meaning a series of bytes will represent each value, the 
reading and interpretation of which can be done without need for any other ancillary information. The only exception to this rule is for the symbol type, and will
be explained in detail later.

Generally speaking, the first byte of each dia-value, henceforth called the type-metadata, represents it's _type_: since dia supports only 9 distinct types, this 
byte is more than sufficient to represent 9 distinct values: 1 - 9. For values of the bool type, a single byte is usually enough to encapsulate the entire data.

The first 4 bits (index 0 - 3) are reserved to represent actual type identifiers, bit 5 (index 4) is reserved for indicating if attributes exist on the value,
bit 6 (index 5) is reserved to indicate if the value is null (if set), while the remaining 2 bits (index 6, 7) are left for each type to use as it pleases.

Immediately after the type-metadata is an optional byte group for attributes, and then a byte group for the types pay-load.

Dia binary packets are a sequential list of dia binary values.

#### _3. Text Representation_
As stated previously, the textual representation of dia is a superset of `json`; it is essentially `json` + attributes + extra-data-types.


### _0. Attribute_
An attribute is not a bonafide dia type, but a special use-case of the [Symbol](#Symbol) type. An attribute is used to tag a value with extra (possible semantic)
meaning, and is usually open to interpretation by the user of the data. Attributes come in 2 flavors:
1. Tags: a simple text defined by the regex: /^[a-zA-Z0-9_-.]\z/
2. Value-Pair: a name/value pair defined as follows: '{tag}@{text}', where {tag} is the name, and {text} is the value. {text} is defined as any valid [Symbol](#Symbol)
   character.
Both of these are valid forms of the [Symbol](#Symbol) type. Dia key-words cannot appear in non-quoted forms of attributes.
  
Note: Attributes are never null (null symbols).

#### _Text Representation_
When present in textual format, attribtues present themselves as text with an end-delimiter of "::". The following examples illustrate this:
```
valid::attributes::<dia-value> // valid
 
in valid::att-ributes::<dia-value> // invalid
  
'scale@metric'::'expiry-notation@false positive'::<dia-value> // valid
  
'genre@horror'::<dia-value> // valid
  
abc@xyz::<dia-value> // invalid
```
  
#### _Binary Representation_
Identical to (Symbol)(#Symbol).

### _1. Bool_
The Bool type represents typical boolean values: `true`, and `false`, in addition to `null`.
  
#### _Text Representation_
Binary values are represented by case insensitive "true" and "false", and the case sensitive "null.bool".
  
#### _Binary Representation_

##### _Type-Metadata Byte_
- `[.... 0001]` (0x1)

##### _Description_
The type-metadata byte is sufficient for encapsulating all instances of the bool value. So a bool value will
be represented with only 1 byte.

- true: `[.1.. 0010]`
- false: `[.0.. 0010]`
- null: `[..1. 0010]`
- attributed: `[...1 0010]`


### _2. Int_
The int type represents signed, unlimited mathematical integer values, in addition to `null`.

#### _Text Representation_
Integers come in 3 flavors:
1. Decimal integers: represented as an optional negative sign, and a series of digits possibly separated by an underscore.
2. Hex integers: represented by the prefix "0x", followed by an unlimited sequence of any valid hexadecimal characters (0-9, a-f, A-F), possibly separated by an underscore.
3. Binary integers: represented by the prefix "0b", followed by an unlimited sequence of zeros and ones, possibly separated by an underscore.
Example:
```
// decimal integers
0 // valid
01 // valid
1 // valid
000 // valid
34565454 // valid
343_454_53 // valid
_545 // invalid
4345_ // invalid

// hex integers
0x01 // valid
0X0 // valid
0x // invalid
0x000a // valid
0xA // valid
0xx // invalid
0x545yt345 // invalid

// binary
0b // invalid
0B0 // invalid
0b0000011010110 // invalid
0b11010110 .. valid
```

#### _Binary Representation_

### Type-Metadata
- `[.... 0010]` (0x2)

### Custom-Metadata
- attributed: `[...1 0010]`
- null: `[..1. 0010]`
- Int8: `[00.. 0010]`
- Int16: `[01.. 0010]`
- Int32: `[10.. 0010]`
- Intxx: `[11.. 0010]`

### Description
Integers require additional bytes besides the type-metadata for their data to be represented.

Dia supports unlimited/arbitrary-length integers, so special consideration is taken to cater for this. 

### Examples
1. To represent the value '42' as an Int8 value, we need 2 bytes:
   - 00 [00.. 0010]
   - 01 [0010 1010]
2. To represent the value '1228' as an Int16 value, we need 3 bytes:
   - 00 [01.. 0010]
   - 01 [1100 1100]
   - 02 [0000 0100]
3. To represent the value '86443187' as an Int32 value, we need 5 bytes:
   - 00 [10.. 0010]
   - 01 [1011 0011]
   - 02 [0000 0100]
   - 03 [0010 0111]
   - 04 [0000 0101]
4. To represent the arbitrarily large integers, e.g '9223372036854775807000981123', we need 15 bytes:
   - 00 [11.. 0010]
   - 01 [1000 0011]
   - 02 [1101 1101]
   - 03 [1101 0000]
   - 04 [1010 0011]
   - 05 [1111 1100]
   - 06 [1111 1111]
   - 07 [1111 1111]
   - 08 [1111 1111]
   - 09 [1111 1111]
   - 10 [1111 1111]
   - 11 [1001 0011]
   - 12 [1110 1011]
   - 13 [1101 1100]
   - 14 [0000 0011]
   
See [var-byte](#var-byte) for a better explanation of how `var-bytes` are represented.


### _3. Decimal_
The Decimal type represents signed, unlimited mathematical floating point values, in addition to `null`.

#### _Text Representation_

























