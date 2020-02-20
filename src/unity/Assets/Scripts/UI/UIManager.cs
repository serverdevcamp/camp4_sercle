using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject skillPanel;

    [Header("Skill Info")]
    [SerializeField] private GameObject skillInfoPanel;
    [SerializeField] private Text skillNameText;
    [SerializeField] private Text skillHotKey;
    [SerializeField] private Text skillDescriptionText;

    private List<GameObject> skillButtons = new List<GameObject>();
    private int heroCount;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        heroCount = GameManager.instance.GetMyHeroCount();
        skillInfoPanel.SetActive(false);

        for (int i = 0; i < heroCount; i++)
        {
            Hero myHero = GameManager.instance.GetMyHero(i);
            GameObject skillButton = Instantiate(buttonPrefab, skillPanel.transform);
            skillButton.GetComponent<SkillButtonIndicator>().Initialize(myHero, (data)=>ShowSkillInfo(myHero.GetSkill, i));
            skillButtons.Add(skillButton);
        }

        AlignSkillButtons();
    }

    private void AlignSkillButtons()
    {
        float buttonSizeX = 200;
        float buttonSizeY = 200;

        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (i >= 3)
            {
                Debug.LogError("스킬이 3개보다 많은 경우는 고려하지 않았습니다!");
                continue;
            }

            RectTransform rect = skillButtons[i].GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(i * 0.5f, 0.5f);
            rect.anchorMax = new Vector2(i * 0.5f, 0.5f);
            rect.pivot = new Vector2(i * 0.5f, 0.5f);
            if (i == 0) rect.anchoredPosition = new Vector2(25f, 0);
            else if (i == 1) rect.anchoredPosition = new Vector2(0, 0);
            else if (i == 2) rect.anchoredPosition = new Vector2(-25f, 0);
        }
    }

    private void ShowSkillInfo(Skill skill, int num)
    {
        string hotKey = "?";
        if (num == 0) hotKey = "Q";
        else if (num == 1) hotKey = "W";
        else if (num == 2) hotKey = "E";

        skillNameText.text = skill.skillName;
        skillDescriptionText.text = skill.description;
        skillHotKey.text = "[" + hotKey + "]";

        skillInfoPanel.SetActive(true);
    }

    private void UnshowSkillInfo()
    {
        skillInfoPanel.SetActive(false);
    }
}
