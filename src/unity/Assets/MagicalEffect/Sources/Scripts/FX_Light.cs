using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class FX_Light : MonoBehaviour
	{
		private Light lighter;
		public float Delay = 0.5f;

		void Start ()
		{
			lighter = this.GetComponent<Light>();
		}
	
		
		void Update ()
		{
			if(lighter){
				lighter.intensity = Mathf.Lerp(lighter.intensity,0,Delay);
			}
		}
	}
}
