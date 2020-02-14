/*
 * 하늘에서 영웅 출현하는(낙하) 스크립트 입니다.
 */ 

using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	[RequireComponent (typeof(Rigidbody))]

public class _FX_HeroEmerge : MonoBehaviour
	{
		private void OnEnable()
		{
			//Rigidbody rig = GetComponent<Rigidbody>();
			//if (rig)
			//{
			//	rig.AddForce(-this.transform.up * Force, ForceMode.Impulse);
			//}
		}
		public float Force = 1000;
		void Start ()
		{
			
		}
	}
}
