using UnityEngine;


namespace XEngine.Engine
{
	[ExecuteInEditMode]
	public class ParticleScaler : MonoBehaviour
	{
		public float Scale = 1f;

		private float realScale = 1f;

		private ParticleSystem PS;

		private float realSize;

		private float realSpeed;

		private float realRotation;

		protected void Awake()
		{
			PS = GetComponent<ParticleSystem>();
			if ((Object)(object)PS == null)
			{
				Destroy((Object)(object)this);
				return;
			}
			ParticleSystem.MainModule main = PS.main;
			realSize = main.startSizeMultiplier;
			main = PS.main;
			realSpeed = main.startSpeedMultiplier;
			main = PS.main;
			realRotation = main.startRotationMultiplier;
			SetScale();
		}

		protected void Update()
		{
			SetScale();
		}

		private void SetScale()
		{
			if (Scale < 0f)
			{
				Scale = 0f;
			}
			if (realScale != Scale)
			{
				realScale = Scale;
				ParticleSystem.MainModule main = PS.main;
				main.startSizeMultiplier = Scale * realSize;
				main.startSpeedMultiplier = Scale * realSpeed;
				main.startRotationMultiplier = Scale * realRotation;
			}
		}
	}
}