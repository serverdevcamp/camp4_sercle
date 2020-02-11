using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MatchingClock : MonoBehaviour
{
    private float matchingTime = 0f;
    private Text timeText;

    private void Start()
    {
        timeText = transform.GetComponentInChildren<Text>();
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        Debug.Log("On Enable");
        matchingTime = 0f;
        transform.DORotate(new Vector3(0, 0, 90), 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void Update()
    {
        matchingTime += Time.deltaTime;
        timeText.text = matchingTime.ToString("0");
        timeText.transform.rotation = Quaternion.identity;
    }
}
