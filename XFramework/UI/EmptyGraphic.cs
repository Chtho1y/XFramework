using UnityEngine.UI;


namespace XEngine.UI
{
	public class EmptyGraphic : MaskableGraphic
	{
		protected EmptyGraphic()
		{
			this.useLegacyMeshGeneration = false;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
