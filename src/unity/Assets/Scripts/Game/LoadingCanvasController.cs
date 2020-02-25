using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour
{
    [Header("UI Holder")]
    [SerializeField] private Text firstCampUserName;
    [SerializeField] private Text secondCampUserName;
    [SerializeField] private Transform firstCampPanel;
    [SerializeField] private Transform secondCampPanel;

    [Header("Prefab")]
    [SerializeField] private GameObject firstCampSkillPrefab;
    [SerializeField] private GameObject secondCampSkillPrefab;

    private void Start()
    {
        List<int> firstCampSkills;
        List<int> secondCampSkills;

        if (GameManager.instance.MyCampNum == 1)
        {
            firstCampSkills = SkillManager.instance.firstCampSkills;
            secondCampSkills = SkillManager.instance.secondCampSkills;
        }
        else
        {
            firstCampSkills = SkillManager.instance.secondCampSkills;
            secondCampSkills = SkillManager.instance.firstCampSkills;
        }
        

        InstantiateFirstCamp(firstCampSkills);
        InstantiateSecondCamp(secondCampSkills);
    }

    private void InstantiateFirstCamp(List<int> skills)
    {
        string jsonFile = Resources.Load<TextAsset>("Json/SkillInfoJson").ToString();
        SkillInfoJsonArray skillInfos = JsonUtility.FromJson<SkillInfoJsonArray>(jsonFile);
        float offset = 100f;

        for (int i = 0; i < skills.Count; i++)
        {
            SkillInfoJson skill = skillInfos.skillInfo[skills[i]];

            GameObject selectedSkill = Instantiate(firstCampSkillPrefab, firstCampPanel);
            selectedSkill.GetComponent<LoadingSkill>().Initialize(true, skill.skillName, Resources.Load<Sprite>(skill.skillImagePath));
            selectedSkill.GetComponent<RectTransform>().anchoredPosition = -new Vector2(0, offset + i * selectedSkill.GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    private void InstantiateSecondCamp(List<int> skills)
    {
        string jsonFile = Resources.Load<TextAsset>("Json/SkillInfoJson").ToString();
        SkillInfoJsonArray skillInfos = JsonUtility.FromJson<SkillInfoJsonArray>(jsonFile);
        float offset = 100f;

        for (int i = 0; i < skills.Count; i++)
        {
            SkillInfoJson skill = skillInfos.skillInfo[skills[i]];

            GameObject selectedSkill = Instantiate(secondCampSkillPrefab, secondCampPanel);
            selectedSkill.GetComponent<LoadingSkill>().Initialize(true, skill.skillName, Resources.Load<Sprite>(skill.skillImagePath));
            selectedSkill.GetComponent<RectTransform>().anchoredPosition = -new Vector2(0, offset + i * selectedSkill.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}
