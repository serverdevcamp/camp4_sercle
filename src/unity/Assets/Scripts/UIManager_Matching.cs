using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager_Matching : MonoBehaviour
{
    [Header("Character Prefab")]
    public List<Character> characters;

    [Header("Character Basic Info")]
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private Text characterName;
    [SerializeField] private List<CustomSlider> statusSliders;
    [SerializeField] private List<Image> skillIcons;

    [SerializeField] private Transform circleImage;

    [Header("Character Skill Info")]
    [SerializeField] private GameObject skillInfoPanel;
    [SerializeField] private Text skillName;
    [SerializeField] private Text skillDescription;

    private void Start()
    {
        characterInfoPanel.SetActive(false);
        skillInfoPanel.SetActive(false);

        circleImage.DOLocalRotate(new Vector3(0, 0, 180), 2f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }


    public void ShowCharacterInfo(int num)
    {
        characterInfoPanel.SetActive(true);
        skillInfoPanel.SetActive(false);

        Character character = characters[num];

        characterName.text = "";
        characterName.DOText(character.name, 0.5f);

        for(int i = 0; i < statusSliders.Count; i++)
        {
            int targetValue;
            if (i == 0) targetValue = character.status.MHP;
            else targetValue = (int)character.status.Value((StatusType)(i + 2));

            statusSliders[i].SetValue(targetValue);
        }

        for (int i = 0; i < character.skills.Count; i++)
        {
            Skill skill = character.skills[i];
            skillIcons[i].GetComponent<Button>().onClick.AddListener(delegate { ShowSkillInfo(skill); });
        }
    }

    public void ShowSkillInfo(Skill skill)
    {
        skillInfoPanel.SetActive(true);

        skillName.text = skill.skillName;
        skillDescription.text = skill.description;
    }
}
