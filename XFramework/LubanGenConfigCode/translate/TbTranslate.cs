//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.translate
{ 

public sealed partial class TbTranslate
{
    private readonly Dictionary<string, translate.Translate> _dataMap;
    private readonly List<translate.Translate> _dataList;
    
    public TbTranslate(JSONNode _json)
    {
        _dataMap = new Dictionary<string, translate.Translate>();
        _dataList = new List<translate.Translate>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = translate.Translate.DeserializeTranslate(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<string, translate.Translate> DataMap => _dataMap;
    public List<translate.Translate> DataList => _dataList;

    public translate.Translate GetOrDefault(string key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public translate.Translate Get(string key) => _dataMap[key];
    public translate.Translate this[string key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}