using UnityEngine;
using UnityEngine.UI;


namespace XEngine.UI
{
	[RequireComponent(typeof(PolygonCollider2D))]
	[RequireComponent(typeof(Button))]
	public class Irregular : Image
	{
		private PolygonCollider2D polygon;

		private PolygonCollider2D Polygon
		{
			get
			{
				if ((Object)(object)polygon == (Object)null)
				{
					polygon = ((Component)this).GetComponent<PolygonCollider2D>();
				}
				return polygon;
			}
		}

		public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			RectTransformUtility.ScreenPointToWorldPointInRectangle(this.rectTransform, screenPoint, eventCamera, out Vector3 val);
			return ((Collider2D)Polygon).OverlapPoint(new Vector2(val.x, val.y));
		}
	}
}