using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedSkill : MonoBehaviour
{
    [SerializeField] private Image skillImage;
    [SerializeField] private Text skillName;

    public void Show(SkillInfoJson skill)
    {
        skillImage.sprite = Resources.Load<Sprite>(skill.skillImagePath);
        skillImage.color = Color.white;
        skillName.text = skill.skillName;
    }

    public void Unshow()
    {
        skillImage.sprite = null;
        skillImage.color = Color.clear;
        skillName.text = "";
    }
}