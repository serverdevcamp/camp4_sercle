using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Image myImage;

    private UIManager_SkillSelect UIManager;

    private int skillNum;

    public void Initialize(UIManager_SkillSelect manager, int num, Sprite image)
    {
        UIManager = manager;
        skillNum = num;
        myImage.sprite = image;

        gameObject.AddComponent<Button>().onClick.AddListener(() => { SendSkillInfo(); });
    }

    private void SendSkillInfo()
    {
        UIManager.ShowSkillInfo(skillNum);
    }
}
