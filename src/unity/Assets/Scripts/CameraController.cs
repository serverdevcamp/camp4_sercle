using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Character curCharacter;
    [SerializeField] private Vector3 dir;
    [SerializeField] private float distance;

    private Vector3 targetPosition;
    float maxDis = 15f;
    float minDis = 3f;

    private void Update()
    {
        curCharacter = GameManager.instance.CurCharacter;
        distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * 0.3f, minDis, maxDis);
        dir.y = MappingDirection(distance);

        AdjustCamera();
        transform.DOMove(targetPosition, 0.3f);
    }

    private void AdjustCamera()
    {
        targetPosition = curCharacter.transform.position + dir.normalized * distance;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
    }

    private float MappingDirection(float distance)
    {
        return (distance - minDis) * 0.2f;
    }
}
