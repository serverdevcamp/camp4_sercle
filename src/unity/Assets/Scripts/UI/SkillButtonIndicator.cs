using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SkillButtonIndicator : MonoBehaviour
{
    [Header("UI Element")]
    [SerializeField] private Image skillImage;

    private Hero myHero;
    private Skill mySkill;

    public void Initialize(Hero hero, 
        UnityAction<BaseEventData> enterAction,
        UnityAction<BaseEventData> exitAction)
    {
        myHero = hero;
        mySkill = myHero.GetSkill;

        skillImage.sprite = mySkill.Image;

        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener(enterAction);
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener(exitAction);
        trigger.triggers.Add(pointerExit);
    }

    private void Update()
    {
        if(myHero != null) ShowRemainCool();
    }

    private void ShowRemainCool()
    {
        float value = 1 - mySkill.RemainCool / mySkill.coolDown;
        skillImage.fillAmount = value;
    }
}
