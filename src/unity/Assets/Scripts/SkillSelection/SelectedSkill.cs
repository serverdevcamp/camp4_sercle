using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectedSkill : MonoBehaviour
{
    [Header("Before Select")]
    [SerializeField] private Text defaultDescription;

    [Header("After Select")]
    [SerializeField] private Image skillImage;
    [SerializeField] private Text skillName;
    [SerializeField] private Text skillDescription;
    [SerializeField] private GameObject cancelBtn;

    public void Initialize(int num, UnityAction action)
    {
        string defaultStr = "SELECT  ";
        if (num == 0) defaultStr += "Q";
        else if (num == 1) defaultStr += "W";
        else if (num == 2) defaultStr += "E";
        else
        {
            Debug.LogError("번호에 해당하는 키를 추가해야합니다.");
        }
        defaultStr += "  SKILL";

        defaultDescription.text = defaultStr;
        cancelBtn.GetComponent<Button>().onClick.AddListener(action);

        Unshow();
    }

    public void Show(SkillInfoJson skill)
    {
        defaultDescription.gameObject.SetActive(false);
        skillImage.sprite = Resources.Load<Sprite>(skill.skillImagePath);
        skillImage.color = Color.white;
        skillName.text = skill.skillName;
        skillDescription.text = skill.skillDesc;
        cancelBtn.SetActive(true);
    }

    public void Unshow()
    {
        defaultDescription.gameObject.SetActive(true);
        skillImage.sprite = null;
        skillImage.color = Color.clear;
        skillName.text = "";
        skillDescription.text = "";
        cancelBtn.SetActive(false);
    }
}