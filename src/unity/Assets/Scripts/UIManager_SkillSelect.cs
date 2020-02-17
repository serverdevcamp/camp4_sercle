using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject selectedSkillPrefab;

    private int? currentSkill = null;
    private List<int> selectedSkills = new List<int>();
    private List<SkillButton> skillButtons = new List<SkillButton>();
    private List<SelectedSkill> mySkillImages = new List<SelectedSkill>();

    private SkillInfoJsonArray skill;

    private void Start()
    {
        InstantiateSkillButton();
        InstantiateMySkillPanel();
    }

    private void Update()
    {
        UpdateSkillButton();
        ShowSelectedSkills();
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
            skillButtons.Add(skillButton.GetComponent<SkillButton>());
        }
    }

    private void InstantiateMySkillPanel()
    {
        for (int i = 0; i < holdableSkillCnt; i++)
        {
            GameObject mySkillImage = Instantiate(selectedSkillPrefab, mySkillPanel);
            RectTransform rect = mySkillImage.GetComponent<RectTransform>();
            Vector2 spawnPos = new Vector2(0, -i * rect.sizeDelta.y);
            rect.anchoredPosition = spawnPos;

            mySkillImage.GetComponent<SelectedSkill>().Unshow();
            mySkillImages.Add(mySkillImage.GetComponent<SelectedSkill>());
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
    }

    private void ShowSelectedSkills()
    {
        for (int i = 0; i < mySkillImages.Count; i++)
        {
            try
            {
                int skillNum = selectedSkills[i];
                mySkillImages[i].Show(skill.skillInfo[skillNum]);
            }
            catch
            {
                mySkillImages[i].Unshow();
            }
        }
    }

    private void UpdateSkillButton()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            //currentSkill의 경우 OnClicked() 실행
            if (i == currentSkill) skillButtons[i].OnClicked();
            //selectedSkills에 포함된 skill의 경우 OnSelected() 실행
            else if (selectedSkills.Contains(i)) skillButtons[i].OnSelected();
            //나머지는 원상태로
            else skillButtons[i].OnIdle();
        }
    }

    public void StartGame()
    {
        Debug.Log("게임 씬으로 넘어갑니다.");
        SendSelectionInfo();
        SceneManager.LoadScene("EQ_Test");
    }

    public void SendSelectionInfo()
    {
        // 스킬 3개 선택 안했을 경우 나머지 스킬은 -1으로 채운다.
        //for (int i = 0; i < selectedSkills.Count; i++)
        //{
        //    selectedSkillIndex.Add(-1);
        //}

        SelectedSkillData data = new SelectedSkillData();
        data.skillIndex = new int[3];

        data.userCamp = MatchingManager.instance.userInfo.userData.playerCamp; // userData.id가 only Integer라는 것을 가정.

        for (int i = 0; i < 3; i++)
        {
            data.skillIndex[i] = selectedSkills[i];
        }

        SelectedSkillPacket packet = new SelectedSkillPacket(data);
        GameObject.Find("GameNetworkManager").GetComponent<GameNetworkManager>().SendLocalSkillSelect(packet);
    }
}
