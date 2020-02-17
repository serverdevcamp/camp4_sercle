using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager_SkillSelect : MonoBehaviour
{
    [Header("For Test")]
    [SerializeField] private Sprite testImage;

    [Header("UI Holder")]
    [SerializeField] private Transform skillPanel;
    [SerializeField] private Text selectedSkillName;
    [SerializeField] private Text selectedSkillDescription;

    [Header("Prefab Holder")]
    [SerializeField] private GameObject skillButtonPrefab;

    private int? currentSkill = null;
    private List<int> selectedSkills = new List<int>();

    private void Start()
    {
        InstantiateSkillButton();
    }

    private void InstantiateSkillButton()
    {
        int rowCnt = 5;

        for (int i = 0; i < 20; i++)
        {
            GameObject skillButton = Instantiate(skillButtonPrefab, skillPanel);
            RectTransform rect = skillButton.GetComponent<RectTransform>();
            Vector2 spawnPos = new Vector2((i % rowCnt) * rect.sizeDelta.x, -(i / rowCnt) * rect.sizeDelta.y);
            rect.anchoredPosition = spawnPos;

            // 번호에 따른 스킬의 이미지 할당
            Sprite skillImage = testImage;
            skillButton.GetComponent<SkillButton>().Initialize(this, i, skillImage);
        }
    }

    public void ShowSkillInfo(int skillNum)
    {
        currentSkill = skillNum;
        // selectedSkillName.text = ;
        // selectedSkillDescription.text = ;
    }

    public void SelectSkill()
    {
        if (currentSkill.HasValue)
        {
            selectedSkills.Add(currentSkill.Value);
            string str = "현재 선택된 스킬들은 ";
            foreach (int skill in selectedSkills)
            {
                str += skill + " ";
            }
            str += "입니다.";

            Debug.Log(str);
        }
    }

    public void StartGame()
    {
        Debug.Log("게임 씬으로 넘어갑니다.");
    }
}
