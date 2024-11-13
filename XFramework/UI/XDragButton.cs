using XEngine.Engine;
using UnityEngine;
using UnityEngine.EventSystems;


namespace XEngine.UI
{
	[RequireComponent(typeof(DragButton))]
	public class XDragButton : XComponent<DragButton>
	{
		protected override void Awake()
		{
			base.Awake();
			base.Target.onDrag = Drag;
			base.Target.onDown = Down;
			base.Target.onUp = Up;
		}

		private void Drag(PointerEventData eventData)
		{
			SendEvent("Drag", eventData);
		}

		private void Down(PointerEventData eventData)
		{
			SendEvent("Down", eventData);
		}

		private void Up(PointerEventData eventData)
		{
			SendEvent("Up", eventData);
		}
	}
}