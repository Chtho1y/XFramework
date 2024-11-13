using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;


namespace XEngine.Engine
{
	public class GameManager : MonoSingleton<GameManager>
	{
		private const float GCStep = 1f;

		private float GCTime = 0f;

		private Camera UICamera;

		private XLuaSimulator luaSimulator;

		protected override void OnSingletonAwake()
		{
			this.hideFlags = HideFlags.HideInInspector;
			Screen.sleepTimeout = -1;
			luaSimulator = new XLuaSimulator();
			UICamera = GameUtil.Find<Camera>("UICamera");
		}

		protected void Update()
		{
			luaSimulator.OnUpdate();
			if (Time.time - GCTime > GCStep)
			{
				luaSimulator?.Update();
				GCTime = Time.time;
			}
		}

		protected void FixedUpdate()
		{
			luaSimulator.OnFixedUpdate();
		}

		protected void LateUpdate()
		{
			luaSimulator.OnLateUpdate();
		}

		private void OnSceneChanged(Scene S, LoadSceneMode M)
		{
			luaSimulator.OnSceneChanged(S.name);
		}

		internal void OnException(string appException)
		{
			StopAllCoroutines();
			luaSimulator.OnException(appException);
		}

		private void OnLowMemory()
		{
			luaSimulator.OnLowMemory();
		}

		protected void OnApplicationFocus(bool focus)
		{
			luaSimulator.OnApplicationFocus(focus);
		}

		protected void OnApplicationPause(bool pause)
		{
			luaSimulator.OnApplicationPause(pause);
		}

		protected void OnApplicationQuit()
		{
			luaSimulator.OnApplicationQuit();
		}

		public void OnCheckUpdateFinished()
		{
			luaSimulator.OnCheckUpdateFinished();
		}

		public void InitLua()
		{
			byte[] gameManagerLuaCode = BundleManager.Instance.LoadLua("Luas/GameManager.lua");

			if (luaSimulator == null || gameManagerLuaCode == null)
			{
				Debug.LogError("LuaSimulator or gameManagerLuaCode is null");
			}
			else
			{
				luaSimulator.DoString(gameManagerLuaCode, "GameManager", null);
				luaSimulator.BindLifecycle();
				SceneManager.sceneLoaded += OnSceneChanged;
				Application.lowMemory += new Application.LowMemoryCallback(OnLowMemory);
				luaSimulator.OnGameEnter();
			}
		}

		public void LoadScene(string sceneName, bool fromBundle = false, Action onLoaded = null, Action<float> onLoading = null)
		{
			if (fromBundle)
			{
				BundleManager.Instance.LoadAllDependencies("Scenes");
				BundleManager.Instance.LoadBundle("Scenes");
			}
			StartCoroutine(BundleManager.Instance.LoadSceneAsync(sceneName, onLoaded, onLoading));
		}

		public Sprite GetSprite(string atlasPath, string name)
		{
			SpriteAtlas sa = BundleManager.Instance.Load<SpriteAtlas>(atlasPath);
			if (sa != null)
			{
				return sa.GetSprite(name);
			}
			return null;
		}

		public Sprite[] GetSprites(string atlasPath)
		{
			SpriteAtlas sa = BundleManager.Instance.Load<SpriteAtlas>(atlasPath);
			if (sa != null)
			{
				Sprite[] array = (Sprite[])(object)new Sprite[sa.spriteCount];
				sa.GetSprites(array);
				return array;
			}
			return null;
		}

		public UnityEngine.Object Load(string path, Type type)
		{
			return BundleManager.Instance.Load(path, type);
		}

		public void LoadAsync(string path, Type type, Action<UnityEngine.Object> handler)
		{
			StartCoroutine(BundleManager.Instance.LoadAsync(path, type, handler));
		}

		public Camera GetUICamera()
		{
			return Instance.UICamera;
		}

		public Camera GetMainCamera()
		{
			return Camera.main;
		}

		public Vector3 World2UIPos(Vector3 worldPos, Camera worldCamera)
		{
			Camera uICamera = Instance.UICamera;
			if (uICamera == null || worldCamera == null)
			{
				return Vector3.zero;
			}
			Vector3 val = worldCamera.WorldToScreenPoint(worldPos);
			Vector3 result = uICamera.ScreenToWorldPoint(val);
			result.z = uICamera.transform.position.z + uICamera.nearClipPlane + 0.01f;
			return result;
		}

		public Coroutine WaitTime2Do(float seconds, Action handler)
		{
			return StartCoroutine(WaitTime(seconds, handler));
		}

		public Coroutine WaitFrame2Do(int frameCount, Action handler)
		{
			return StartCoroutine(WaitFrame(frameCount, handler));
		}

		private IEnumerator WaitTime(float seconds, Action handler)
		{
			yield return new WaitForSeconds(seconds);
			handler?.Invoke();
		}

		private IEnumerator WaitFrame(int frameCount, Action handler)
		{
			do
			{
				yield return new WaitForEndOfFrame();
				frameCount--;
			}
			while (frameCount > 0);
			handler?.Invoke();
		}

		public void CancelWait2Do(Coroutine waitCoroutine)
		{
			StopCoroutine(waitCoroutine);
		}

		public void CancelAllWait2Do()
		{
			StopAllCoroutines();
		}

		//优点，使用Unity事件调度函数Invoke()，性能优秀，高效	
		//缺陷，只能同时完成一个动作的调用，有新的调用会清空正在进行的任务
		private Action delayAction;
		private void InvokeMethod()
		{
			delayAction?.Invoke();
			delayAction = null;
		}

		public void WaitInvoke(float delay, Action handler)
		{
			if (delayAction != null)
			{
				CancelInvoke();
			}
			delayAction += handler;
			Invoke(nameof(InvokeMethod), delay);
		}

		private Action repeatAction;
		private bool isRepeating = false;

		public void RepeatInvoke(float delay, float repeatRate, Action action)
		{
			if (isRepeating)
			{
				CancelInvoke(nameof(RepeatMethod));
			}

			repeatAction = action;
			isRepeating = true;
			InvokeRepeating(nameof(RepeatMethod), delay, repeatRate);
		}

		private void RepeatMethod()
		{
			repeatAction?.Invoke();
		}

		public void StopRepeat()
		{
			if (isRepeating)
			{
				CancelInvoke(nameof(RepeatMethod));
				isRepeating = false;
				repeatAction = null;
			}
		}

		public string GetDeviceID()
		{
			string deviceID = string.Empty;

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver"))
                {
                    using (AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure"))
                    {
                        deviceID = settingsSecure.CallStatic<string>("getString", contentResolver, "android_id");
                    }
                }
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        deviceID = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_EDITOR
			deviceID = "EditorDeviceID";
#else
        deviceID = SystemInfo.deviceUniqueIdentifier;
#endif
			return deviceID;
		}

	}
}