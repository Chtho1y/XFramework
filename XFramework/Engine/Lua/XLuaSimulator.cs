using UnityEngine;
using XLua;


namespace XEngine.Engine
{
	public partial class XLuaSimulator
	{
		private static LuaEnv _luaEnv = null;

		public XLuaSimulator()
		{
			Init();
		}

		protected internal void Init()
		{
			// 这里 LuaEnv 中包含了 Lua 的全局环境，可以在这里添加一些全局变量
			_luaEnv = new LuaEnv();
			_luaEnv.AddLoader(CustomLoader);
		}

		protected internal void Update()
		{
			_luaEnv?.Tick();
		}

		protected internal void Dispose()
		{
			UnbindLifecycle();
			_luaEnv?.Dispose();
			_luaEnv = null;
		}

		protected internal object[] DoString(byte[] bytes, string chunk, LuaTable table)
		{
			return _luaEnv.DoString(bytes, chunk, table);
		}

		protected internal byte[] CustomLoader(ref string path)
		{
			// 这里的path是Lua代码中require的参数 比如 Common.DataStructure.Queue
			if (BundleManager.Instance == null)
			{
				Debug.Log("Lua Load Error: BundleManager Instance is Null");
				return null;
			}
			var luaRelativePath = "Luas/" + path.Replace(".", "/") + ".lua";
			return BundleManager.Instance.LoadLua(luaRelativePath);
		}

		private void BindLuaFunction2Csharp<T>(string luaFuncName, ref T csharpFunc)
		{
			csharpFunc = _luaEnv.Global.GetInPath<T>(luaFuncName);
		}
	}
}
