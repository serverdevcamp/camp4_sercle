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
    private float speed = 50f;
    private Vector3 LeftDir = Vector3.left;
    private Vector3 RightDir = Vector3.right;
    private Vector3 UpDir = Vector3.forward;
    private Vector3 DownDir = Vector3.back;

    private void Start()
    {
        angle.x = defaultAngle;
        angle.y = GameManager.instance.MyCampNum == 1 ? 0 : 180;
        distance = defualtDis;
        
        targetPosition = Vector3.zero;

        int campNum = GameManager.instance.MyCampNum;
        if (campNum == 2)
        {
            LeftDir = Vector3.right;
            RightDir = Vector3.left;
            UpDir = Vector3.back;
            DownDir = Vector3.forward;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) targetPosition += LeftDir * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) targetPosition += RightDir * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) targetPosition += UpDir * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) targetPosition += DownDir * speed * Time.deltaTime;

        transform.DORotate(new Vector3(angle.x, angle.y, 0), 0.3f);
        transform.DOMove(targetPosition - new Vector3(0, Mathf.Sin(angle.x), -Mathf.Cos(angle.x)) * distance, 0.1f).SetEase(Ease.Linear);
    }
}
