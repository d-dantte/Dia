# Dia
> Data representation specification and implementation, supporting both textual (human readable) and binary formats.


## Contents
1. [Introduction](#Introduction)
2. [Specification](#Specification)
3. [Attribute](#Attribute)
4. [Bool](#Bool)
5. [Integer](#Int)
6. [Decimal](#Decimal)
7. [Instant](#Instant)
8. [String](#String)
9. [Symbol](#Symbol)
10. [Clob](#Clob)
11. [Blob](#Blob)
12. [List](#List)
13. [Record](#Record)
14. [Appendix](#Appendix) (things like key-words, var-bytes, etc will appear here)


## <a id="Introduction"></a> Introduction
Dia is yet another data representation format, born from the need for more feature-sets in json; as such, it is a superset of json.

## <a id="Specification"></a> Specification
Dia recognizes 9 data types. The concept of types in `Dia` allow for the absence of values: in this case, a null is used. `Dia` types are:
0. Attribute
1. Bool
2. Int
3. Decimal
4. Instant
5. String
6. Symbol
7. Clob
8. Blob
9. List
10. Record

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



### <a id="Attribute"></a> _0. Attribute_
An attribute is not a bonafide dia type, but a special use-case of the [Symbol](#-6-symbol) type. An attribute is used to tag a value with extra (possible semantic)
meaning, and is usually open to interpretation by the user of the data. Attributes come in 2 flavors:
1. Tags: a simple text defined by the regex: /^[a-zA-Z0-9_-.]\z/
2. Value-Pair: a name/value pair defined as follows: '&lt;tag&gt;@&lt;text&gt;', where &lt;tag&gt; is the name, and &lt;text&gt; is the value. &lt;text&gt; is defined as any valid [Symbol](#Symbol)
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



### <a id="Bool"></a> _1. Bool_
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



### <a id="Int"></a> _2. Integer_
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

##### _Type-Metadata_
- `[.... 0010]` (0x2)

##### _Custom-Metadata_
- attributed: `[...1 0010]`
- null: `[..1. 0010]`
- Int8: `[00.. 0010]`
- Int16: `[01.. 0010]`
- Int32: `[10.. 0010]`
- Intxx: `[11.. 0010]`

##### _Description_
Integers require additional bytes besides the type-metadata for their data to be represented.

Dia supports unlimited/arbitrary-length integers, so special consideration is taken to cater for this. 

##### _Examples_
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



### <a id="Decimal"></a> _3. Decimal_
The Decimal type represents signed, unlimited mathematical floating point values, in addition to `null`.

#### _Text Representation_
Decimals come in 2 flavors:
1. Regular decimal notation
2. Scientific/exponent decimal notation.
Examples:
```
// regular notation
0.0
123.0
-0.44544

// exponent notation
0.0e0
2.5E-1
2345.0E+06
```
With either notation, the fractional part of the number must always be present. Also, the exponent notation
is essentially the regular notation with a "E&lt;sign&gt;&lt;digits&gt;" concatenated to it.

#### _Binary Representation_

##### _Type-Metadata_
- `[.... 0011]` (0x3)

##### _Custom-Metadata_
- Attributed: `[...1 0011]`
- null: `[..1. 0011]`
- Decimal16: `[00.. 0011]`
- BigDecimal: `[01.. 0011]`

##### _Description_
The binary representation for decimals comes in 2 flavors:
1.  The straightforward adaptation of the `dotnet` binary representation for decimal, ergo 16 bytes of data.
2.  The custom format for big (arbitrary size) decimal values. BigDecimals are a tuple of an int (scale), and an 
    arbitrary-length integer (significand) represented by a [var-byte](#var-byte) value.



### _4. <a id="Instant"></a> Instant_
The Instant type represents a timestamp, in addition to `null`. Instants are represented in various precisions, with
the unspecified precisions assuming the default values.

#### _Text Representation_
In text, instants are represented in the format: &lt;year&gt;-&lt;month&gt;-&lt;day&gt;&lt;delimiter&gt;&lt;hour&gt;:&lt;minute&gt;:&lt;second&gt;.&lt;sub-seconds&gt;.

- Mandatory components: year, month, day, delimiter
- Optional components: hour, minute, second, sub-seconds. Optional components are used in order of thier appearance: e.g, `year-month-day-delimiter-second.sub-second` is invalid.
Examples:
```
2023-02-13T // <year>-<month>-<day><delimiter> -> 2023/02/13 00:00:00.0
1993-09-27T12 // <year>-<month>-<day><delimiter><hour> -> 1993/09/27 12:00:00.0
1993-09-27T12:31 // <year>-<month>-<day><delimiter><hour>:<minute> -> 1993/09/27 12:31:00.0
1993-09-27T12:31:08 // <year>-<month>-<day><delimiter><hour>:<minute>:<seconds> -> 1993/09/27 12:31:08.0
1993-09-27T12:31:08.0023319 // <year>-<month>-<day><delimiter><hour>:<minute>:<seconds>.<sub-second> -> 1993/09/27 12:31:08.0023319
```

#### _Binary Representation_

##### _Type-Metadata_
- `[.... 0100]` (0x4)

##### _Custom-Metadata_
- Attributed: `[...1 0100]`
- null: `[..1. 0100]`
- HMS: `[.1.. 0100]`
- Sub-seconds: `[1... 0100]`

##### _Description_
The instant is made of 7 components, each with their own binary representation, the first 3 of which are mandatory.
In practice, however, there are actually only 5 components, because the `Hour`, `Minute` and `Second` components
are stored as a unit of seconds. Presence of the optional components are indicated by the 2 custom-metadata bits
as shown above.

Components include (in the order they appear in the data stream):
1. Year
2. MD (Month-Day)
3. HMS (Hour-Minute-Seconds) (optional)
4. Sub-seconds (optional)

###### _Year_
The year is a special component. It represents an ever increasing value, and as such a definit amount of data cannot
be reserved for it. Owing to this, it is stored using `var-bytes`. The unique ability for `var-bytes` to store an
arbitrary stream of bits is taken advantage of, thus the year component also acts as a "bit-lender" for other components
whose data representations overflow whole-bytes.
Specifically speaking, the `Day` component borrows the needed 1 bit from the first bit of the `Year` component: meaning
a single "right-shift" of the bits restores the original value of the year component.

* Data type: var-byte
* Capacity: variable
* Arrangement: 
    * year[0]: `Day` component overflow
    * year[1..*]: `Year` data.


###### _Month + Day_
12 months require 4 bits to encapsulate, while 31 days require 5 bits. Together, this is a byte and one bit. As stated
above, the single bit is borrowed from the first bit-position of the `year` component.

* Data type: byte
* Capacity: 1
* Arrangement:
    * MD[0..3]: `Month` data
    * MD[4..7]: `Day` data
    * Year[0] : `Day` data


###### _Hour + Minute + Second_
There are a total of _86,400_ seconds in a day, translating to exactly 17 bits (2 bytes and a bit) of data. Considering the
optional nature of this component, the possibility of the values swinging from a single byte all the way to 3 bytes, this
component will be represented using [var-byte](#Var-byte).

* Data type: var-byte
* Capacity: variable
* Arrangement:
    * HMS[0..*]: `HMS` data


###### _Sub-seconds_
The unit of the subsecond component is the [tick](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.ticks?view=net-7.0#remarks),
each of which is 100 nanoseconds. There are a total of 9,999,999 ticks in a second, translating to 24 bits, or 3 bytes.
Again, this will be represented by a [var-byte](#Var-byte), owing to the potential sparse nature of the data.

* Data type: var-byte
* Capacity: variable
* Arrangement:
    * HMS[0..*]: `Sub-second` data



### <a id="String"></a> _5. String_
The string maintains its ubiquitous definition here: a sequence of unicode-encoded characters, in addition to `null`.

#### _Text Representation_
There are 2 flavors of this: Single-line string, Multi-line string. Both representations are delimiter-enclosed, and support escaping.

##### _1. Single-line string_
This is represented as a delimiter-enclosed group of characters that must be presented on a single line. This means literal characters that
descend to the next line must be escaped. The enclosing delimiter for the single-line string is `"`.
Examples:
```
"a valid string"

"another valid string with \n escaped new-line"

"another valid string with \u11EC a unicode escaped character"

"invalid
string"
```

##### _2. Multi-line string_
This slightly more complex flavor supports new-line characters, and epecial escape sequences that allow for user-friendly representation
of text. The enclosing delimiter for the multi-line string is `@"`, and `"`.
Exmaples:
```
@"Valid string"

@"Valid string
with new line"

@"Valid string
with new line, and another \n escaped new ilne"

// special escaping for improved formatting/readability
@"\
        This string has the special new-line escape that \
        essentially \"swallows\" all white-space characters \
        up until the next non-whitespace character, or the \
        end of the text. to use a regular back-slash, do this \\
        in this case, all of the white space preceeding the regular \
        back-slash are included in the text.
"
```

##### <a id="Escapes"></a> _Escape sequences_
The following escape sequences are supported:

| Escape Sequence | Meaning                                                                                     |
|:----------------|:--------------------------------------------------------------------------------------------|
| \0              | Null character                                                                              |
| \a              | Bell                                                                                        |
| \b              | Backspace                                                                                   |
| \t              | Tab                                                                                         |
| \v              | Vertical Tab                                                                                |
| \n              | New line                                                                                    |
| \r              | Carriage return                                                                             |
| \f              | Form feed                                                                                   |
| \\"             | Double quote                                                                                |
| \\\\            | Backslash                                                                                   |
| \NL             | Escape whitespaces. A backslash followed by a new line, and arbitrary number of whitespaces |
| \xHH            | 1 byte char escape                                                                          |
| \uHHHH          | 2 byte char escape                                                                          |
| \UHHHHHHHH      | 4 byte char escape                                                                          |

#### _Binary Representation_

### Type-Metadata
- `[.... 0101]` (0x5)

### Custom-Metadata
- Attributed: `[...1 0101]`
- null: `[..1. 0101]`

### Description
String data is stored as unicode - ie, 2 bytes per character; however, a string-count component is used to signify
how many characters (groups of 2 bytes) the string contains. The string-count comes right after the type-metadata,
and is represented as a [var-byte](#Var-byte).



### <a id="Symbol"></a> _6. Symbol_
A symbol is similar to a string, but with a few restrictions on it. It is a sequence of printable ascii characters  and escape sequences.
When the sequence of characters conforms to the pattern `/^\[a-zA-Z_\](([.-])?\[a-zA-Z0-9_\])*\z/`, the symbol is called an `identifier`.
In the identifier form, symbols must exclude the Dia [keywords](#Keywords). Two symbols are equivalent if they contain the same sequence
of characters.

#### _Textual Representation_
Textually, symbols are enclused in the `'` delimiter. When they are `identifiers`, the single-quotes can be omited, and when present, cannot
be empty. 

Escape sequences supported are all listed [here](#Escapes), excluding `\NL`, but including `\'`.

Exmaples:
```
null.symbol // valid, representing a null symbol
abc // valid identifier symbol

'abc' // valid identifier symbol, equivalent to the previous symbol

abc_xyz // valid

abc.xyz // valid

abc-xyz // valid

symbol // invalid

symbol.something.else // valid

null // invalid (keyword)

null_something // valid

null.something // valid

'another valid symbol' // valid

'also valid \n\x4e \u1c2f symbol' // valid
```

#### _Binary Representation_
Symbols represent the only data types that need contextual information while reading/writing from a binary stream. The reason is that symbols
are repeated a lot, and can benefit from some form of compression. The compression process used is simple:
1. Differentiate between a binary representation of a regular symbol, and a symbol ID.
2. The first time a regular symbol is encountered, it is read/written as a regular symbol, and an ID is created for it using a sequentially
   incremented integer value, and store in a symbol table. This table is built and used ONLY during the course of reading/writing.
3. For reading scenarios, when a symbol ID is encountered, it means it has already been read before, so the actual value is resolved from the
   table mentioned above.
4. For writing scenarios, subsequent encounters of the symbol will be resolved from the table, and the IDs will be written to the stream.

##### _Type-Metadata_
- `[.... 0110]` (0x6)

##### _Custom-Metadata_
- Attributed: `[...1 0110]`
- null: `[..1. 0110]`
- regular symbol: `[.... 0110]`
- symbol ID: `[.1.. 0110]`

##### _Description_
In the same manner as with [strings](#String), data is store as unicode, so the symbol uses a [var-byte](#Var-byte) to store the character count
(__not byte count__), and following that is a sequence of bytes for the characters.



### <a id="Clob"></a> _7. Clob_
The clob is similar to the symbol: It is a sequence of ascii characters. Clobs can be used for all sorts of data representations - a good
example is for representing and transmitting scripts, or other textual infomration.

#### _Text Representation_
Clob values are represented textually as a delimiter-enclosed sequence of printable ascii characters, and escape sequences. Enclosing delimiters
are `<<` and `>>`

Escape sequences supported are all listed [here](#Escapes), excluding `\NL`, but including `\>`.

Examples:
```
<<
    clob stuff. Can be anything at all, however, use of the greater-than symbol \> must be escaped.
>>
```

#### _Binary Representation_

### Type-Metadata
- `[.... 0111]` (0x7)

### Custom-Metadata
- Attributed: `[...1 0111]`
- null: `[..1. 0111]`

### Description
Following the type-metadata is a `var-byte` value that represents the number of expected bytes - since Clobs are ascii characters,
each character is one byte. Following the `var-byte` value is the actual byte sequence for the Clob.



### <a id="Blob"></a> _8. Blob_
The blob is, as the name implies, a block of raw bytes.

#### _Text Representation_
Blob values are represented textually as a delimiter-enclosed base-64 encoding of the actual bytes. Enclosing delimiters
are `<` and `>`. Whitespaces and [comments](#Comments) are allowed between the delimiters and the base-64 data.

Examples:
```
<
    VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4=
>
<VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4=>
< VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZy4= >
```

#### _Binary Representation_

### Type-Metadata
- `[.... 1000]` (0x8)

### Custom-Metadata
- Attributed: `[...1 1000]`
- null: `[..1. 1000]`

### Description
Following the type-metadata is a `var-byte` value that represents the number of expected bytes, following that, is the actual
byte sequence for the Blob.



### <a id="List"></a> _9. List_
A list is a sequence of zero or more `Dia` values.

#### _Text Representation_
Similar to json, this is represented as a delimiter enclosed sequence of comma separated values. Delimiters are `[`, and `]`.
Between each value can appear whitespaces or [comments](#Comments).

Examples:
```
[ 234, 3.45, true, [], 1992-04-05T, <abcxyz> << clob text>>, attributed::"string value"]
```

#### _Binary Representation_

### Type-Metadata
- `[.... 1001]` (0x9)

### Custom-Metadata
- Attributed: `[...1 1001]`
- null: `[..1. 1001]`

### Description
Similar to the [blob](#ion-blob), a `var-byte` value follows the type-metadata, representing the number of items in the list.
Following the count, are the binary representation of each value, layed out serially.



### <a id="Record"></a> _10. Record_
A record is a sequence of zero or more `Dia` properties. A property is a key-value pair, where the key is a non-null symbol,
and the value is any legal `Dia` value. Records do not support duplicated property names in it's sequence of properties.

#### _Text Representation_
Similar to json, this is represented as a delimiter enclosed sequence of comma separated properties. Delimiters are `{`, and `}`.
Between each property can appear whitespaces or [comments](#Comments). Each property in turn consists of a [symbol](#Symbol) and
any dia-value, separated by a colon `:`. Again, whitespaces/comments can appear anywhere between these elements.

Worthy of note is that the symbols in this case can also be enclosed in `"` - this way, valid `json` also become valid `dia` structures.

Examples:
```
// valid
{ something: true}

// valid
{
    something: 2345.54,
    attribute::"key" : < b64_bytes= >,
    'Key@value'::again::'property' : bleh::34
}
```

#### _Binary Representation_

### Type-Metadata
- `[....-1010]` (0xA)

### Custom-Metadata
- Attributed: `[...1 1010]`
- null: `[..1. 1010]`

### Description
The record is similar to the [list](#List), i.e the `var-byte` count represents number of properties in the record.
Following the property count, the properties are themselves laid out serially with the key/name first, then the value
last.



### <a id="Appendix"></a> _Apendix_

#### <a id="Var-byte"></a> _Var-byte_
A variable byte binary representation. This is a regular 1-byte integer number, except that the sign-bit now represents
'overflow': if the bit is set, it means another [var-byte](#var-byte) value follows, containing more bits for the data.
Reading the collection of `var-byte` data requires removing all the overflow bits, and concatenating the remaining bits.


### <a id="Appendix"></a> _Comments_
Comments are...
