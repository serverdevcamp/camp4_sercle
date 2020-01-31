using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    [SerializeField] private int vertexCnt = 40;
    [SerializeField] private float lineWidth = 0.5f;

    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        SetupCircle();
    }

    public void SetupCircle(float radius = 0f)
    {
        lineRenderer.widthMultiplier = lineWidth;

        float deltaTheta = (2f * Mathf.PI) / vertexCnt;
        float theta = 0f;

        lineRenderer.positionCount = vertexCnt;
        for (int i = 0; i < vertexCnt; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}
