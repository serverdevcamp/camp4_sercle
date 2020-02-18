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
    [SerializeField] private GameObject selectedSkillPrefab;

    private void Start()
    {
        List<int> firstCampSkills = SkillManager.instance.mySkills;
        List<int> secondCampSkills = SkillManager.instance.enemySkills;

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

            GameObject selectedSkill = Instantiate(selectedSkillPrefab, firstCampPanel);
            selectedSkill.GetComponent<LoadingSkill>().Initialize(true, skill.skillName, Resources.Load<Sprite>(skill.skillImagePath));
            selectedSkill.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset + i * selectedSkill.GetComponent<RectTransform>().sizeDelta.y, 0);
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

            GameObject selectedSkill = Instantiate(selectedSkillPrefab, secondCampPanel);
            selectedSkill.GetComponent<LoadingSkill>().Initialize(true, skill.skillName, Resources.Load<Sprite>(skill.skillImagePath));
            selectedSkill.GetComponent<RectTransform>().anchoredPosition = new Vector2(offset + i * selectedSkill.GetComponent<RectTransform>().sizeDelta.y, 0);
        }
    }
}
