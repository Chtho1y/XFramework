using XEngine.Engine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XDropdown : XComponent<Dropdown>
	{
		public int value
		{
			get
			{
				return base.Target.value;
			}
			set
			{
				base.Target.value = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onValueChanged).RemoveAllListeners();
			((UnityEvent<int>)(object)base.Target.onValueChanged).AddListener((UnityAction<int>)ValueChanged);
		}

		private void ValueChanged(int value)
		{
			SendEvent("ValueChanged", value);
		}
	}
}