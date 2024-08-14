//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;


namespace cfg
{ 
   
public sealed partial class Tables
{
    public translate.TbTranslate TbTranslate {get; }
    public uiFormConfig.TbUIFormConfig TbUIFormConfig {get; }
    public audioConfig.TbAudioConfig TbAudioConfig {get; }
    public miscellaneousConfig.TbMiscellaneousConfig TbMiscellaneousConfig {get; }
    public entityConfig.TbEntityConfig TbEntityConfig {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbTranslate = new translate.TbTranslate(loader("translate_tbtranslate")); 
        tables.Add("translate.TbTranslate", TbTranslate);
        TbUIFormConfig = new uiFormConfig.TbUIFormConfig(loader("uiformconfig_tbuiformconfig")); 
        tables.Add("uiFormConfig.TbUIFormConfig", TbUIFormConfig);
        TbAudioConfig = new audioConfig.TbAudioConfig(loader("audioconfig_tbaudioconfig")); 
        tables.Add("audioConfig.TbAudioConfig", TbAudioConfig);
        TbMiscellaneousConfig = new miscellaneousConfig.TbMiscellaneousConfig(loader("miscellaneousconfig_tbmiscellaneousconfig")); 
        tables.Add("miscellaneousConfig.TbMiscellaneousConfig", TbMiscellaneousConfig);
        TbEntityConfig = new entityConfig.TbEntityConfig(loader("entityconfig_tbentityconfig")); 
        tables.Add("entityConfig.TbEntityConfig", TbEntityConfig);
        PostInit();

        TbTranslate.Resolve(tables); 
        TbUIFormConfig.Resolve(tables); 
        TbAudioConfig.Resolve(tables); 
        TbMiscellaneousConfig.Resolve(tables); 
        TbEntityConfig.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbTranslate.TranslateText(translator); 
        TbUIFormConfig.TranslateText(translator); 
        TbAudioConfig.TranslateText(translator); 
        TbMiscellaneousConfig.TranslateText(translator); 
        TbEntityConfig.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}