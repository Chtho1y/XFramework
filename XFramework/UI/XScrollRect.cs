using XEngine.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XScrollRect : XComponent<ScrollRect>
	{
		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<Vector2>)(object)base.Target.onValueChanged).AddListener((UnityAction<Vector2>)ValueChanged);
		}

		private void ValueChanged(Vector2 value)
		{
			SendEvent("ValueChanged", value);
		}
	}
}