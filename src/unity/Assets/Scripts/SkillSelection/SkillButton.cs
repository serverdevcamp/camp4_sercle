using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Image myImage;

    private UIManager_SkillSelect UIManager;

    private int skillNum;

    public bool isClicked { get; set; }

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

    public void OnClicked()
    {
        GetComponent<Image>().color = Color.yellow;
        GetComponent<Button>().interactable = true;
        myImage.color = Color.white;
    }

    public void OnSelected()
    {
        GetComponent<Image>().color = Color.white;
        GetComponent<Button>().interactable = false;
        myImage.color = new Color(1, 1, 1, 0.3f);
    }

    public void OnIdle()
    {
        GetComponent<Image>().color = Color.white;
        GetComponent<Button>().interactable = true;
        myImage.color = Color.white;
    }
}
