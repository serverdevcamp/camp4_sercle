using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 dir;
    [SerializeField] private float distance;
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;
    [SerializeField] private float defualtDis;
    
    private Vector3 targetPosition;
    private Vector3 moveDir = Vector3.zero;
    private float moveSpeed = 20f;


    private void Start()
    {
        distance = 15f;
    }

    private void Update()
    {
        distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * 0.3f, minDis, maxDis);

        targetPosition += moveDir * moveSpeed * Time.fixedDeltaTime;

        transform.DOMove(targetPosition + dir.normalized * distance, 0.3f);
    }
}
