using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI : MonoBehaviour
{
    private float speed;
    private bool isRight = false;

    private void Start()
    {
        StartCoroutine(Patrol());
    }

    private void Update()
    {
        speed = GetComponent<Character>().status.SPD;

        Vector3 dir = isRight ? Vector3.right : Vector3.left;

        transform.position += dir * speed * Time.deltaTime;
    }

    private IEnumerator Patrol()
    {
        yield return new WaitForSeconds(1f);

        isRight = !isRight;

        StartCoroutine(Patrol());
    }
}
