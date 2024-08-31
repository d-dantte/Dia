# Axis.Dia.PathQuery

## Ideas

- Similar to Pulsar Symbol Path query.
- The logical structure is: Query -> Path -> Segment -> Matcher
- Query: ??
- Path: Is a list of consecutive segments, where each segment is applied to the list of set of `IDiaValue` instances available in the query context
- Segment: A segment represents filter/matcher logic to be applied to the current dataset. Matcher types include
  - Property Name pattern: applied to property names of the given dataset. This filter is presented as a subset of regular expressions.
  - Index range pattern: applied to indexes of sequences of the given dataset. Presented in the manner of c# ranges. Where single values are used, it depicts a single index.
  - Type pattern: applied to any instance in the dataset to filter out specific `DiaType` instances.
  - Attribute pattern: applied to any instance in the dataset. Filters instances that contain attributes that match the given pattern
  
  In addition to the matcher types, segments include support for a negation, where a logical negation is applied to whatever matcher logis is specified
- for the segment. This is depicted with a `!` appearing right after the segment delimiter `/`

### Name Pattern
```
/:abcd - find properties with name "abcd"
/!:abcd - find properties where name is NOT "abcd"
```


&nbsp;
### Index Range Pattern
```
/#2..^1 - find all elements at indexes 2 to 'sequence length - 1'
/!#2.. - find all elements at indexes 
```


&nbsp;
### Type Qualifier Pattern
```
/$Record $Integer - match if the given value is of type "Record" or "Integer"
/! $Record $Integer - match if the given value is not of type "Record", or "Integer". Note that spaces can appear anywhere between the type specifiers
```



&nbsp;
### Attribute Pattern
```
/@abc; @bleh:`helb``; - match if the given value has the given attribtues
/! @abc; @bleh:`helb`; - match if the given value does not have the given attribtues
```

When matching text, 2 predicates are supported:
- Wildcard Predicate: this matches character-by-character till the entire match is complete. It recognizes a special `_` character used to denote "any" character while matching.
  Wildcards also support the concept of cardinalities - where a token or group of tokens can be designated a cardinality that specifies how many repetitions the group is permitted
  for a match to be recognized.

- Regular expression predicate: this is, as the name implies, the standard regular expression.


#### Wildcard
```
abcd
a{2}bc(d_f){?}.123{1,4} - where available, cardinalities are applied to either the character or group they preceed.

```