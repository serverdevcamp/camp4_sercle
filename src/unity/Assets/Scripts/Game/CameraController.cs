using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float defaultAngle;
    [SerializeField] private float defualtDis;
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;

    [SerializeField] private float distance;
    [SerializeField] private float angle;

    private Vector3 targetPosition;

    private void Start()
    {
        angle = defaultAngle;
        distance = defualtDis;
        
        targetPosition = Vector3.zero;
    }

    private void Update()
    {
        transform.DORotate(new Vector3(angle, 0, 0), 0.3f);
        transform.DOMove(targetPosition + new Vector3(0, Mathf.Sin(defaultAngle), -Mathf.Cos(defaultAngle)) * distance, 0.3f);
    }
}
