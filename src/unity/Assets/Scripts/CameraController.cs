using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Character curCharacter;
    [SerializeField] private Vector3 dir;
    [SerializeField] private float distance;

    private void Update()
    {
        distance -= Input.mouseScrollDelta.y * 0.3f;
        dir.y = MappingDirection(distance);

        AdjustCamera();
    }

    private void AdjustCamera()
    {
        transform.position = curCharacter.transform.position + dir.normalized * distance;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
    }

    private float MappingDirection(float distance)
    {
        float maxDis = 15f;
        float minDis = 3f;

        return (distance - minDis) * 0.2f;
    }
}
