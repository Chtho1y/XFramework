using UnityEngine;


namespace XEngine.Main
{
    public static class BundleServerConfig
    {
        public static readonly string ServerUrl = Application.platform switch
        {
            // don't forget to add '/'
            RuntimePlatform.WindowsEditor => @"",
            RuntimePlatform.OSXEditor => @"",
            RuntimePlatform.Android => @"",
            RuntimePlatform.IPhonePlayer => @"",
            _ => "Server not configured for this platform",
        };
    }
}
