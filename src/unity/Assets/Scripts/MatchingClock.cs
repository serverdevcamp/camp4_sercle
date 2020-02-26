using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MatchingClock : MonoBehaviour
{
    private float matchingTime = 0f;
    private Text timeText;

    private string oldTime;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            oldTime = "0";
            matchingTime = 0f;
        }
        else if(SceneManager.GetActiveScene().name == "Skill Select")
        {
            oldTime = "30";
            matchingTime = 30f;
        }

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
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            matchingTime += Time.deltaTime;
            timeText.text = matchingTime.ToString("0");
            timeText.transform.rotation = Quaternion.identity;
            if (oldTime != timeText.text)
            {
                SoundManager.instance.PlaySound("Lobby_Timer", 0.8f);
                oldTime = timeText.text;
            }
        }
        else if(SceneManager.GetActiveScene().name == "Skill Select" && !transform.GetComponentInParent<UIManager_SkillSelect>().isSelectionFinished)
        {
            matchingTime -= Time.deltaTime;
            timeText.text = matchingTime.ToString("0");
            timeText.transform.rotation = Quaternion.identity;
            if (oldTime != timeText.text)
            {
                // 10초 이하로 남았을 때 재촉 사운드 재생.
                if (matchingTime <= 10f)
                {
                    SoundManager.instance.PlaySound("Lobby_Timer", 0.8f);
                    timeText.color = Color.red;
                    transform.GetComponent<Image>().color = Color.red;
                }
                oldTime = timeText.text;
            }

            if(matchingTime <= 0f)
            {
                // 스킬 강제선택 후 서버로 전송.
                transform.GetComponentInParent<UIManager_SkillSelect>().StartGame();
            }
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
