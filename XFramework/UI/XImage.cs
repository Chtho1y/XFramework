using XEngine.Engine;
using UnityEngine;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XImage : XComponent<Image>
	{
		public Sprite sprite
		{
			get
			{
				return base.Target.sprite;
			}
			set
			{
				base.Target.sprite = value;
			}
		}

		public void SetSprite(Sprite sprite)
		{
			base.Target.sprite = sprite;
		}
	}
}