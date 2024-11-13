using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T instance;
	private static readonly object lockObject = new object();

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				return null;
			}
			return instance;
		}
	}

	public static void InitializeInstance(T newInstance)
	{
		if (instance == null)
		{
			instance = newInstance;
			DontDestroyOnLoad(newInstance.gameObject);
		}
	}

	protected virtual void Awake()
	{
		lock (lockObject)
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this as T;

			if (instance.transform.root.name == instance.name) DontDestroyOnLoad(gameObject);
			OnSingletonAwake();
		}
	}

	protected virtual void OnSingletonAwake() { }

	public virtual void Dispose()
	{
		if (instance == this)
		{
			Destroy(this);
			instance = null;
		}
	}
}

public class SingletonInitializer
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void CreateSingleton<T>() where T : MonoSingleton<T>, new()
	{
		if (MonoSingleton<T>.Instance == null)
		{
			GameObject singletonObject = new GameObject($"{typeof(T).Name} (Singleton)");
			T instance = singletonObject.AddComponent<T>();
			Object.DontDestroyOnLoad(singletonObject);
			MonoSingleton<T>.InitializeInstance(instance);
		}
	}
}