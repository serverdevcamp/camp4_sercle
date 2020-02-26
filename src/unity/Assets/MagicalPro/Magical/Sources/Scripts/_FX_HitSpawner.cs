using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class _FX_HitSpawner : MonoBehaviour
	{


		public GameObject FXSpawn;
		public bool DestoyOnHit = false;
		public bool FixRotation = false;
		public float LifeTimeAfterHit = 1;
		public float LifeTime = 0;
        private bool isMeteor;

		void Start ()
		{
		
		}
	
		void Spawn ()
		{
			if (FXSpawn != null) {
				Quaternion rotate = this.transform.rotation;
				if (!FixRotation)
					rotate = FXSpawn.transform.rotation;
				GameObject fx = (GameObject)GameObject.Instantiate (FXSpawn, this.transform.position, rotate);
                
				if (LifeTime > 0)
					GameObject.Destroy (fx.gameObject, LifeTime);
			}
			if (DestoyOnHit) {
			
				GameObject.Destroy (this.gameObject, LifeTimeAfterHit);
				if (this.gameObject.GetComponent<Collider>())
					this.gameObject.GetComponent<Collider>().enabled = false;

			}
		}
	
		void OnTriggerEnter (Collider other)
		{
            if(transform.name == "Meteo")
            {
                if (isMeteor)
                    return;
            }
			Spawn ();
            isMeteor = true;
		}
	
		void OnCollisionEnter (Collision collision)
		{
            if (transform.name == "Meteo")
            {
                if (isMeteor)
                    return;
            }
            Spawn();
            isMeteor = true;
        }
	}
}