using XEngine.Engine;
using UnityEngine.UI;


namespace XEngine.UI
{
	public class XText : XComponent<Text>
	{
		public string Key;

		public string text
		{
			get
			{
				return base.Target.text;
			}
			set
			{
				base.Target.text = value;
			}
		}

		public void SetText(string text)
		{
			base.Target.text = text;
		}
	}
}