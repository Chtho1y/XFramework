using XEngine.Engine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XToggle : XComponent<Toggle>
	{
		public bool isOn
		{
			get
			{
				return base.Target.isOn;
			}
			set
			{
				base.Target.isOn = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<bool>)(object)base.Target.onValueChanged).AddListener((UnityAction<bool>)ValueChanged);
		}

		private void ValueChanged(bool value)
		{
			SendEvent("ValueChanged", value);
		}
	}
}