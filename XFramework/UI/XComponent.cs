using UnityEngine;
using XEngine.Engine;


namespace XEngine.UI
{
	public abstract class XComponent<T> : MonoBehaviour where T : Component
	{
		private T target;

		public T Target
		{
			get
			{
				if ((Object)(object)target == null)
				{
					target = GetComponent<T>();
				}
				return target;
			}
		}

		protected virtual void Awake()
		{
			target = GetComponent<T>();
		}

		protected virtual void OnDestroy()
		{
			UnityEventHelper.RemoveEventsOn(this);
		}

		protected void SendEvent(string name, params object[] data)
		{
			UnityEventHelper.SendEvent(this, name, data);
		}

		public virtual void Dispose()
		{
		}
	}
}
