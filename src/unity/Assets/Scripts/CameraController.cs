using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 dir;
    [SerializeField] private float distance;

    private Character focusCharacter;
    private Vector3 targetPosition;
    private Vector3 moveDir = Vector3.zero;
    private float moveSpeed = 20f;
    float maxDis = 15f;
    float minDis = 3f;

    private void Start()
    {
        focusCharacter = null;
    }

    private void Update()
    {
        distance = Mathf.Clamp(distance - Input.mouseScrollDelta.y * 0.3f, minDis, maxDis);
        dir.y = MappingDirection(distance);

        if(focusCharacter) targetPosition = focusCharacter.transform.position;

        targetPosition += moveDir * moveSpeed * Time.fixedDeltaTime;

        transform.DOMove(targetPosition + dir.normalized * distance, 0.3f);
    }

    public void FocusCharacter(Character character)
    {
        focusCharacter = character;
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
    }

    private float MappingDirection(float distance)
    {
        return (distance - minDis) * 0.2f;
    }

    public void MoveCamera(string direction, bool start)
    {
        if (!start)
        {
            moveDir = Vector3.zero;
            return;
        }

        focusCharacter = null;
        float angle = Mathf.Abs(transform.rotation.eulerAngles.y);

        switch (direction)
        {
            case "up":
                angle += Mathf.PI / 2;
                break;
            case "down":
                angle -= Mathf.PI / 2;
                break;
            case "left":
                angle += Mathf.PI;
                break;
            case "right":
                break;
            default:
                Debug.LogError("You spelled wrong...");
                return;
        }

        moveDir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }
}
