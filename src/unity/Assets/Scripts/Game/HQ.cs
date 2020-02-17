using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : Robot
{
    protected override void Start()
    {
        status.ChangeStatTo(StatusType.CHP, status.MHP);
    }

    protected override void Update()
    {
        if (status.CHP <= 0)
        {
            if(Is1P) Debug.Log("1P의 패배!");
            else Debug.Log("2P의 패배!");

            Destroy(gameObject);
        }
    }
}
