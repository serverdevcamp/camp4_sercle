using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginAnimation : MonoBehaviour
{
    public Transform canon;
    public ParticleSystem canonBall;
   
    private void LateUpdate()
    {
        //Debug.Log(canon.localEulerAngles);
        if (canon.localEulerAngles.x == 347.6f)
        {
            if (!canonBall.isPlaying)
                canonBall.Play();
        }
        else
            canonBall.Stop();
    }
}
