using XEngine.Engine;
using UnityEngine;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XRawImage : XComponent<RawImage>
	{
		public Texture texture
		{
			get
			{
				return base.Target.texture;
			}
			set
			{
				base.Target.texture = value;
			}
		}

		public void SetTexture(Texture texture)
		{
			base.Target.texture = texture;
		}
	}
}
