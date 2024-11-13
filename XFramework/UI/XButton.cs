using XEngine.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace XEngine.UI
{
	[RequireComponent(typeof(Button))]
	public class XButton : XComponent<Button>
	{
		[SerializeField]
		private float coolTime = 0.1f;

		private float curTime = 0f;

		private bool isCooling = false;

		protected override void Awake()
		{
			base.Awake();
			((UnityEventBase)base.Target.onClick).RemoveAllListeners();
			((UnityEvent)base.Target.onClick).AddListener(new UnityAction(Click));
		}

		private void Click()
		{
			if (!isCooling)
			{
				SendEvent("Click");
				isCooling = true;
				curTime = 0f;
			}
		}

		protected void Update()
		{
			if (isCooling)
			{
				curTime += Time.deltaTime;
				if (curTime > coolTime)
				{
					isCooling = false;
					curTime = 0f;
				}
			}
		}

		public void SetCoolTime(float coolTime)
		{
			if (coolTime < 0f)
			{
				coolTime = 0f;
			}
			this.coolTime = coolTime;
		}
	}
}