using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillButtonIndicator : MonoBehaviour
{
    [Header("UI Element")]
    [SerializeField] private Image skillImage;

    private Hero myHero;
    private Skill mySkill;

    public void Initialize(Hero hero, UnityEngine.Events.UnityAction<BaseEventData> enterAction)
    {
        myHero = hero;
        mySkill = myHero.GetSkill;

        skillImage.sprite = mySkill.Image;

        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener(enterAction);
        trigger.triggers.Add(pointerEnter);
    }

    private void Update()
    {
        if(myHero != null) ShowRemainCool();
    }

    public void ActivateSkill()
    {
        if (mySkill.RemainCool == 0)
        {
            Debug.Log("스킬을 활성화 합니다. 실제로는 아니지만... ㅋㅋ");
            ShowSkillInfo();
        }
        else Debug.Log("스킬이 쿨타임 중이네요?");
        
    }

    public void ShowSkillInfo()
    {
        Debug.Log("스킬에 관련된 정보를 보여줍니다. 실제로는 아니구...^^");
    }

    public void UnshowSkillInfo()
    {
        Debug.Log("스킬에 관련된 정보창을 끕니다.");
    }

    private void ShowRemainCool()
    {
        float value = 1 - mySkill.RemainCool / mySkill.coolDown;
        skillImage.fillAmount = value;
    }
}
