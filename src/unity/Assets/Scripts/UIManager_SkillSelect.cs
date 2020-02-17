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
    [SerializeField]
    private List<GameObject> skillButtonList = new List<GameObject>();

    private SkillInfoJsonArray skill;

    private void Start()
    {
        InstantiateSkillButton();
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
            skillButtonList.Add(skillButton);
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
        if (currentSkill.HasValue)
        {
            // 이미 기존에 선택한 스킬이었던 경우에는 리스트에 추가하지 않는다.
            if(selectedSkills.Contains(currentSkill.Value))
            {
                selectedSkillName.text = "SYSTEM";
                selectedSkillDescription.text = skill.skillInfo[currentSkill.Value].skillName + " 은(는) 이미 선택한 스킬입니다.\n다른 스킬을 선택해 주세요.";
                return;
            }

            selectedSkills.Add(currentSkill.Value);
            skillButtonList[currentSkill.Value].GetComponent<SkillButton>().isClicked = true;

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
