using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class _FX_ShakeCamera : MonoBehaviour
	{

		public Vector3 Power = Vector3.up;

		void Start ()
		{
			_CameraEffect.Shake (Power);
		}
	}
}
