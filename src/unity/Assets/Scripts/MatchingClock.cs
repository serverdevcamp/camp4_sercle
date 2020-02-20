using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MatchingClock : MonoBehaviour
{
    private float matchingTime = 0f;
    private Text timeText;

    private string oldTime;

    private void Start()
    {
        oldTime = "0";
        timeText = transform.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        transform.DORotate(new Vector3(0, 0, 100f), 1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

    private void Update()
    {
        matchingTime += Time.deltaTime;
        timeText.text = matchingTime.ToString("0");
        timeText.transform.rotation = Quaternion.identity;
        if(oldTime != timeText.text)
        {
            SoundManager.instance.PlaySound("Lobby_Timer", 0.8f);
            oldTime = timeText.text;
        }
    }

    private void OnDisable()
    {
        oldTime = "0";
        timeText.text = "0";
        matchingTime = 0;
        transform.DOKill();
        transform.rotation = Quaternion.identity;
    }
}
