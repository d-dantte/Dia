using Axis.Dia.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axis.Dia.Utils
{

    /// <summary>
    /// NOTE:
    /// 1. null-symbols should NEVER be added to this hash-list
    /// 2. annotations MUST be stripped away from symbols before performing any operation on them in this class
    /// </summary>
    //public class SymbolHashList : IEnumerable<SymbolValue>
    //{
    //    private readonly Dictionary<int, SymbolValue> _isMap = new Dictionary<int, SymbolValue>();
    //    private readonly Dictionary<SymbolValue, int> _siMap = new Dictionary<SymbolValue, int>();

    //    public int Count => _isMap.Count;

    //    public bool IsEmpty => Count == 0;

    //    public SymbolHashList()
    //    {
    //    }

    //    public SymbolHashList(IEnumerable<SymbolValue> types)
    //    : this(types?.ToArray() ?? throw new ArgumentNullException(nameof(types)))
    //    {
    //    }

    //    public SymbolHashList(params SymbolValue[] types)
    //    {
    //        if (types == null)
    //            throw new ArgumentNullException(nameof(types));

    //        foreach (var item in types)
    //            Add(item);
    //    }

    //    public bool Remove(SymbolValue value)
    //    {
    //        value = StripAnnotations(value);

    //        if (!_siMap.ContainsKey(value))
    //            return false;

    //        var index = _siMap[value];

    //        _siMap.Remove(value);
    //        _isMap.Remove(index);

    //        return true;
    //    }

    //    public bool Remove(int index)
    //    {
    //        if (!_isMap.ContainsKey(index))
    //            return false;

    //        var value = _isMap[index];

    //        _siMap.Remove(value);
    //        _isMap.Remove(index);

    //        return true;
    //    }

    //    public int Add(SymbolValue value)
    //    {
    //        value = StripAnnotations(value);
    //        _ = TryAdd(value, out var index);
    //        return index;
    //    }

    //    /// <summary>
    //    /// Adds the given symbol to the list.
    //    /// </summary>
    //    /// <param name="value">The symbol to add</param>
    //    /// <param name="index">It's index within the list</param>
    //    /// <returns>True if the symbol was absent before this call, otherwise false</returns>
    //    public bool TryAdd(SymbolValue value, out int index)
    //    {
    //        value = StripAnnotations(value);

    //        // value already exists, return false, and out it's index
    //        if (_siMap.TryGetValue(value, out index))
    //            return false;

    //        // add the value
    //        index = _siMap.Count;

    //        _siMap.Add(value, index);
    //        _isMap.Add(index, value);

    //        return true;
    //    }

    //    public SymbolValue this[int index]
    //    {
    //        get => _isMap[index];
    //    }

    //    public int IndexOf(SymbolValue value)
    //    {
    //        value = StripAnnotations(value);

    //        return _siMap.TryGetValue(value, out var index)
    //            ? index
    //            : -1;
    //    }

    //    public bool Contains(SymbolValue value)
    //    {
    //        value = StripAnnotations(value);
    //        return _siMap.ContainsKey(value);
    //    }

    //    public bool TryGetSymbol(int index, out SymbolValue symbol)
    //    {
    //        if (!_isMap.TryGetValue(index, out symbol))
    //            return false;

    //        return true;
    //    }

    //    public bool TryGetIndex(SymbolValue symbol, out int index)
    //    {
    //        if (!_siMap.TryGetValue(symbol, out index))
    //            return false;

    //        return true;
    //    }

    //    public bool TryGetSymbolID(
    //        SymbolValue symbol,
    //        out IonSymbolPayload.IonSymbolID id)
    //    {
    //        id = TryGetIndex(symbol, out var index)
    //            ? new IonSymbolPayload.IonSymbolID(index)
    //            : default;

    //        return !id.IsNull;
    //    }

    //    private SymbolValue StripAnnotations(SymbolValue symbol)
    //    {
    //        if (symbol.Annotations.Length == 0)
    //            return symbol;

    //        return new SymbolValue(symbol.Value);
    //    }

    //    #region IEnumerable
    //    public IEnumerator<SymbolValue> GetEnumerator() => _siMap.Keys.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //    #endregion
    //}
}
