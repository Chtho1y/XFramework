namespace XEngine.Engine
{
	public abstract class Singleton<T> where T : Singleton<T>, new()
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				instance ??= new();
				return instance;
			}
		}

		public virtual void Init()
		{
		}

		public virtual void Dispose()
		{
		}
	}
}