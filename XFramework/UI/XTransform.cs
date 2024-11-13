using UnityEngine;


namespace XEngine.UI
{
	public class XTransform : XComponent<Transform>
	{
		public new GameObject gameObject => Target.gameObject;

		public void SetActive(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}