using System;
using UnityEngine.EventSystems;


namespace XEngine.UI
{
	public class DragButton : UIBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler, IPointerUpHandler
	{
		public Action<PointerEventData> onDrag { get; set; } = null;


		public Action<PointerEventData> onDown { get; set; } = null;


		public Action<PointerEventData> onUp { get; set; } = null;


		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			onDrag(eventData);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			onDown?.Invoke(eventData);
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			onUp?.Invoke(eventData);
		}

		protected override void OnDestroy()
		{
			onDrag = null;
			onDown = null;
			onUp = null;
			this.OnDestroy();
		}
	}
}