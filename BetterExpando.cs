using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace champ
{
  public class BetterExpando : DynamicObject
  {
    private Dictionary<string, object> _dict;
    private bool _ignoreCase;

    public BetterExpando(bool ignoreCase = false)
    {
      _dict = new Dictionary<string, object>();
      _ignoreCase = ignoreCase;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      UpdateDictionary(binder.Name, value);
      return true;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
    {
      if (indexes[0] is string)
      {
        var key = indexes[0] as string;
        UpdateDictionary(key, value);
        return true;
      }
      else
      {
        return base.TrySetIndex(binder, indexes, value);
      }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      var key = _ignoreCase ? binder.Name.ToLower() : binder.Name;
      if (_dict.ContainsKey(key))
      {
        result = _dict[key];
        return true;
      }
      return base.TryGetMember(binder, out result);
    }

    public dynamic Augment(BetterExpando obj)
    {
      obj._dict
        .Where(pair => !_dict.ContainsKey(pair.Key))
        .ForEach(pair => UpdateDictionary(pair.Key, pair.Value));
      return this;
    }

    public bool HasProperty(string name)
    {
      return _dict.ContainsKey(name);
    }

    public override string ToString()
    {
      return String.Join(", ", _dict.Select(pair => pair.Key + " = " + pair.Value).ToArray());
    }

    private void UpdateDictionary(string name, object value)
    {
      var key = _ignoreCase ? name.ToLower() : name;
      if (_dict.ContainsKey(key))
      {
        _dict[key] = value;
      }
      else
      {
        _dict.Add(key, value);
      }
    }
  }
}
