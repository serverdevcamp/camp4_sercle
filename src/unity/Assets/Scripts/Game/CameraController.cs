using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float defaultAngle;
    [SerializeField] private float maxAngle;
    [SerializeField] private float minAngle;
    [SerializeField] private float defualtDis;
    [SerializeField] private float maxDis;
    [SerializeField] private float minDis;

    [SerializeField] private float distance;
    [SerializeField] private Vector2 angle;

    private Vector3 targetPosition;

    private void Start()
    {
        angle.x = defaultAngle;
        angle.y = GameManager.instance.MyCampNum == 1 ? 0 : 180;
        distance = defualtDis;
        
        targetPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) targetPosition += new Vector3(-50, 0, 0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) targetPosition += new Vector3(50, 0, 0) * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) targetPosition += new Vector3(0, 0, 50) * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) targetPosition += new Vector3(0, 0, -50) * Time.deltaTime;

        transform.DORotate(new Vector3(angle.x, angle.y, 0), 0.3f);
        transform.DOMove(targetPosition - new Vector3(0, Mathf.Sin(angle.x), -Mathf.Cos(angle.x)) * distance, 0.1f).SetEase(Ease.Linear);
    }
}
