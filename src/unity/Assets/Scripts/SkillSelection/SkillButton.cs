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

    public delegate void OnSelected(bool isClicked);
    private OnSelected selectedAction;

    public void Initialize(UIManager_SkillSelect manager, int num, Sprite image)
    {
        UIManager = manager;
        skillNum = num;
        myImage.sprite = image;

        gameObject.AddComponent<Button>().onClick.AddListener(() => { SendSkillInfo(); });
        selectedAction += SetImageTransparent;
    }

    private void SendSkillInfo()
    {
        UIManager.ShowSkillInfo(skillNum);
    }

    private void SetImageTransparent(bool flag)
    {
        // 선택이 되어서 반투명 되어야 하는 경우
        if (flag)
        {
            myImage.color = new Color(1, 1, 1, 0.3f);
        }
        // 선택 해제 되어서 반투명 해제해야 하는 경우
        else
        {
            myImage.color = Color.white;
        }
    }

    private void RegisterSelection(bool flag)
    {
        // 선택 되어서 좌측 이미지에 등록되어야 하는 경우
        if (flag)
        {

        }
        else
        {

        }
    }
}
