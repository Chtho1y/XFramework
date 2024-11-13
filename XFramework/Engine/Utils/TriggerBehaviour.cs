using System;
using UnityEngine;


namespace XEngine.Engine
{
	public class TriggerBehaviour : MonoBehaviour
	{
		public Action<Collider> onTriggerEnter;

		public Action<Collider> onTriggerStay;

		public Action<Collider> onTriggerExit;

		protected void OnTriggerEnter(Collider other)
		{
			onTriggerEnter?.Invoke(other);
		}

		protected void OnTriggerStay(Collider other)
		{
			onTriggerStay?.Invoke(other);
		}

		protected void OnTriggerExit(Collider other)
		{
			onTriggerExit?.Invoke(other);
		}

		protected void OnDestroy()
		{
			onTriggerEnter = null;
			onTriggerStay = null;
			onTriggerExit = null;
		}
	}
}