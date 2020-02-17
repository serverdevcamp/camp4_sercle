using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager_SkillSelect : MonoBehaviour
{
    [SerializeField] private int holdableSkillCnt;

    [Header("UI Holder")]
    [SerializeField] private Transform skillPanel;
    [SerializeField] private Text selectedSkillName;
    [SerializeField] private Text selectedSkillDescription;
    [SerializeField] private Transform mySkillPanel;

    [Header("Prefab Holder")]
    [SerializeField] private GameObject skillButtonPrefab;
    [SerializeField] private GameObject mySkillPrefab;

    private int? currentSkill = null;
    private List<int> selectedSkills = new List<int>();
    private List<GameObject> skillButtons = new List<GameObject>();
    private List<Image> mySkillImages = new List<Image>();

    private SkillInfoJsonArray skill;

    private void Start()
    {
        InstantiateSkillButton();
        InstantiateMySkillPanel();
    }

    private void Update()
    {
        UpdateSkillButton();
    }

    private void InstantiateSkillButton()
    {
        string jsonFile = Resources.Load<TextAsset>("Json/SkillInfoJson").ToString();
        skill = JsonUtility.FromJson<SkillInfoJsonArray>(jsonFile);

        int rowCnt = 5;

        for (int i = 0; i < 20; i++)
        {
            GameObject skillButton = Instantiate(skillButtonPrefab, skillPanel);
            RectTransform rect = skillButton.GetComponent<RectTransform>();
            Vector2 spawnPos = new Vector2((i % rowCnt) * rect.sizeDelta.x, -(i / rowCnt) * rect.sizeDelta.y);
            rect.anchoredPosition = spawnPos;
            skillButton.GetComponent<SkillButton>().Initialize(this, i, Resources.Load<Sprite>(skill.skillInfo[i].skillImagePath));
            skillButtons.Add(skillButton);
        }
    }

    private void InstantiateMySkillPanel()
    {
        for (int i = 0; i < holdableSkillCnt; i++)
        {
            GameObject mySkillImage = Instantiate(mySkillPrefab, mySkillPanel);
            RectTransform rect = mySkillImage.GetComponent<RectTransform>();
            Vector2 spawnPos = new Vector2(0, -i * rect.sizeDelta.y);
            rect.anchoredPosition = spawnPos;

            mySkillImages.Add(mySkillImage.GetComponentInChildren<Image>());
        }
    }

    public void ShowSkillInfo(int skillNum)
    {
        currentSkill = skillNum;
        selectedSkillName.text = skill.skillInfo[skillNum].skillName;
        selectedSkillDescription.text = skill.skillInfo[skillNum].skillDesc;
    }

    public void SelectSkill()
    {
        if (currentSkill.HasValue && selectedSkills.Count < holdableSkillCnt)
        {
            selectedSkills.Add(currentSkill.Value);
            currentSkill = null;
        }

        ShowSelectedSkills();
    }

    private void ShowSelectedSkills()
    {
        for (int i = 0; i < mySkillImages.Count; i++)
        {
            Image skillImage = mySkillImages[i].transform.GetChild(0).GetComponent<Image>();
            try
            {
                int skillNum = selectedSkills[i];
                skillImage.color = Color.white;
                skillImage.sprite = Resources.Load<Sprite>(skill.skillInfo[skillNum].skillImagePath);
            }
            catch
            {
                skillImage.color = Color.clear;
            }
        }
    }

    private void UpdateSkillButton()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            //currentSkill의 경우 OnClicked() 실행
            if (i == currentSkill) skillButtons[i].GetComponent<SkillButton>().OnClicked();
            //selectedSkills에 포함된 skill의 경우 OnSelected() 실행
            else if (selectedSkills.Contains(i)) skillButtons[i].GetComponent<SkillButton>().OnSelected();
            //나머지는 원상태로
            else skillButtons[i].GetComponent<SkillButton>().OnIdle();
        }
    }

    public void StartGame()
    {
        Debug.Log("게임 씬으로 넘어갑니다.");
    }
}
