namespace XEngine.Engine
{
    [XLua.CSharpCallLua]
    public delegate void LuaOnGameEnter();
    [XLua.CSharpCallLua]
    public delegate void LuaOnCheckUpdateFinished();
    [XLua.CSharpCallLua]
    public delegate void LuaOnUpdate();
    [XLua.CSharpCallLua]
    public delegate void LuaOnFixedUpdate();

    [XLua.CSharpCallLua]
    public delegate void LuaOnLateUpdate();

    [XLua.CSharpCallLua]
    public delegate void LuaOnSceneChanged(string sceneName);

    [XLua.CSharpCallLua]
    public delegate void LuaOnException(string e);

    [XLua.CSharpCallLua]
    public delegate void LuaOnLowMemory();

    [XLua.CSharpCallLua]
    public delegate void LuaOnApplicationFocus(bool focus);

    [XLua.CSharpCallLua]
    public delegate void LuaOnApplicationPause(bool pause);
    [XLua.CSharpCallLua]
    public delegate void LuaOnApplicationQuit();


    public partial class XLuaSimulator
    {
        private LuaOnGameEnter onGameEnter;
        private LuaOnCheckUpdateFinished onCheckUpdateFinished;
        private LuaOnUpdate onUpdate;
        private LuaOnFixedUpdate onFixedUpdate;
        private LuaOnLateUpdate onLateUpdate;

        private LuaOnSceneChanged onSceneChanged;
        private LuaOnException onException;
        private LuaOnLowMemory onLowMemory;
        private LuaOnApplicationFocus onApplicationFocus;
        private LuaOnApplicationPause onApplicationPause;
        private LuaOnApplicationQuit onApplicationQuit;

        protected internal void BindLifecycle()
        {
            BindLuaFunction2Csharp("OnGameEnter", ref onGameEnter);
            BindLuaFunction2Csharp("OnCheckUpdateFinished", ref onCheckUpdateFinished);
            BindLuaFunction2Csharp("OnGameUpdate", ref onUpdate);
            BindLuaFunction2Csharp("OnGameLateUpdate", ref onLateUpdate);
            BindLuaFunction2Csharp("OnGameFixedUpdate", ref onFixedUpdate);
            BindLuaFunction2Csharp("OnGameSceneChanged", ref onSceneChanged);
            BindLuaFunction2Csharp("OnGameException", ref onException);
            BindLuaFunction2Csharp("OnGameLowMemory", ref onLowMemory);
            BindLuaFunction2Csharp("OnApplicationFocus", ref onApplicationFocus);
            BindLuaFunction2Csharp("OnApplicationPause", ref onApplicationPause);
            BindLuaFunction2Csharp("OnApplicationQuit", ref onApplicationQuit);
        }

        protected internal void UnbindLifecycle()
        {
            onGameEnter = null;
            onCheckUpdateFinished = null;
            onUpdate = null;
            onLateUpdate = null;
            onFixedUpdate = null;
            onApplicationFocus = null;
            onApplicationPause = null;
            onSceneChanged = null;
            onException = null;
            onLowMemory = null;
            onApplicationQuit = null;
        }

        protected internal void OnGameEnter()
        {
            onGameEnter?.Invoke();
        }

        protected internal void OnCheckUpdateFinished()
        {
            onCheckUpdateFinished?.Invoke();
        }


        protected internal void OnUpdate()
        {
            onUpdate?.Invoke();
        }

        protected internal void OnLateUpdate()
        {
            onLateUpdate?.Invoke();
        }

        protected internal void OnFixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        protected internal void OnApplicationFocus(bool focus)
        {
            onApplicationFocus?.Invoke(focus);
        }

        protected internal void OnApplicationPause(bool pause)
        {
            onApplicationPause?.Invoke(pause);
        }

        protected internal void OnSceneChanged(string sceneName)
        {
            onSceneChanged?.Invoke(sceneName);
        }

        protected internal void OnException(string e)
        {
            onException?.Invoke(e);
        }

        protected internal void OnLowMemory()
        {
            onLowMemory?.Invoke();
        }

        protected internal void OnApplicationQuit()
        {
            onApplicationQuit?.Invoke();
        }

    }

}
