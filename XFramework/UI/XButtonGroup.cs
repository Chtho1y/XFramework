using XEngine.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XButtonGroup : XComponent<Button>
	{
		[SerializeField]
		private Button[] buttons;

		public Button[] Buttons => buttons;

		public int Current { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			int i;
			for (i = 0; i < Buttons.Length; i++)
			{
				Button val = Buttons[i];
				((UnityEventBase)val.onClick).RemoveAllListeners();
				((UnityEvent)val.onClick).AddListener((UnityAction)delegate
				{
					Selected(i);
				});
			}
			Selected(0);
		}

		private void Selected(int selected)
		{
			if (selected != Current)
			{
				SendEvent("Selected", selected);
				Current = selected;
			}
		}
	}
}
