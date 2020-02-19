using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour
{
    [SerializeField] private Robot target = null;
    [SerializeField] private LayerMask contactLayer;

    private Robot robot;
    private int campNum;
    private Attack attack;

    private void Start()
    {
        robot = GetComponent<Robot>();
        campNum = robot.CampNum;
        attack = robot.MyAttack;
    }

    private void Update()
    {
        FindNearestTarget();

        if (target)
        {
            Vector3 dir = target.transform.position - transform.position;
            StartCoroutine(attack.Use(robot, dir));
        }
    }

    private void FindNearestTarget()
    {
        target = null;

        Collider[] colls = Physics.OverlapSphere(transform.position, attack.range, contactLayer);

        float nearestDis = 9999999;

        foreach (Collider coll in colls)
        {
            if (coll.gameObject == gameObject) continue;
            if (campNum == coll.transform.GetComponent<Robot>().CampNum) continue;

            if (target == null || Vector3.Distance(coll.transform.position, transform.position) < nearestDis)
            {
                nearestDis = Vector3.Distance(coll.transform.position, transform.position);
                target = coll.GetComponent<Robot>();
            }
        }
    }
}
