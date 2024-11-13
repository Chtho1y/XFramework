namespace XEngine.Engine
{
	public enum ResourceMode
	{
		Raw, // 直接读本地源文件
		LocalStreamingAssetBundle, // 本地 StreamingAssets 作为AB服务器, 从AB包中读取资源
		Server, // 从服务器下载AB来加载
	}
}
