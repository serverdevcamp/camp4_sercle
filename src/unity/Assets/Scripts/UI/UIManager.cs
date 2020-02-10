﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Current Character Profile")]
    [SerializeField] private Image curHealthBar;
    [SerializeField] private List<Text> statusText;
    [SerializeField] private List<Image> skillCool;

    [Header("Friend Characters Profile")]
    [SerializeField] private List<Image> friendHealthBars;
    [SerializeField] private List<Image> friendQSkillCool;
    [SerializeField] private List<Image> friendWSkillCool;

    [Header("Skill Input Profile")]
    [SerializeField] private Animator skill_Base;
    [SerializeField] private Animator skill_Q;
    [SerializeField] private Animator skill_W;

    private Character curCharacter;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        curCharacter = GameManager.instance.CurCharacter;

        DisplayCharactersInfo(curCharacter);
    }

    // 20 02 10 스킬 발동 버튼이 눌렸을 경우, 그 스킬의 UI를 약간 Scale up한다.
    public void DisplaySkillInputAnimation(int skillNum)
    {
        switch (skillNum)
        {
            case 0:
                skill_Base.SetTrigger("Input");
                break;
            case 1:
                skill_Q.SetTrigger("Input");
                break;
            case 2:
                skill_W.SetTrigger("Input");
                break;
            default:
                break;
        }
    }

    private void DisplayCharactersInfo(Character current)
    {
        if (current.index == 0)
        {
            DisplayCurrentCharacterInfo(0);
            DisplayFriendCharacterInfo(1, 0);
            DisplayFriendCharacterInfo(2, 1);
        }
        else if (current.index == 1)
        {
            DisplayCurrentCharacterInfo(1);
            DisplayFriendCharacterInfo(0, 0);
            DisplayFriendCharacterInfo(2, 1);
        }
        else if (current.index == 2)
        {
            DisplayCurrentCharacterInfo(2);
            DisplayFriendCharacterInfo(0, 0);
            DisplayFriendCharacterInfo(1, 1);
        }
    }

    /// <summary>
    /// 해당하는 캐릭터에 대한 정보를 현재 캐릭터 정보창에 표시
    /// </summary>
    /// <param name="charIndex">현재 캐릭터 번호</param>
    private void DisplayCurrentCharacterInfo(int charIndex)
    {
        curCharacter = GameManager.instance.myCharacters[charIndex];

        // 좌상단 Profile Panel의 HP 표시
        curHealthBar.fillAmount = (float)curCharacter.status.CHP / curCharacter.status.MHP;

        // 좌상단 Profile Panel의 Status 표시
        for (int i = 0; i < statusText.Count; i++)
        {
            statusText[i].text = curCharacter.status.Value((StatusType)(i + 3)).ToString();
        }

        // 중앙하단 Skill Panel의 Skill 쿨타임 표시
        for (int i = 0; i < skillCool.Count; i++)
        {
            Skill skill = curCharacter.skills[i];
            Image image = skillCool[i];

            image.fillAmount = 1f - (skill.RemainCool / skill.coolDown);

            // 20 02 10 이 캐릭터가 CC기에 피격된 상태일 경우, 스킬 쿨타임 UI의 색을 빨간색으로 한다. 
            if(curCharacter.GetCharacterState() == CharacterState.CC)
            {
                image.color = Color.red;
                continue;
            }

            switch (skill.skillState)
            {
                case Skill.SkillState.Idle:
                    image.color = Color.white;
                    break;
                case Skill.SkillState.Ready:
                    image.color = Color.yellow;
                    break;
                case Skill.SkillState.PreDelay:
                    image.color = Color.black;
                    break;
                case Skill.SkillState.CoolDown:
                    image.color = Color.white;
                    break;
            }
        }
    }

    /// <summary>
    /// 아군 캐릭터에 대한 정보를 아군 캐릭터 정보창에 표시
    /// </summary>
    /// <param name="charIndex">아군 캐릭터 번호</param>
    /// <param name="uiIndex">표시할 UI 번호</param>
    private void DisplayFriendCharacterInfo(int charIndex, int uiIndex)
    {
        Character character = GameManager.instance.myCharacters[charIndex];

        Image healthBar = friendHealthBars[uiIndex];
        Image QSkill = friendQSkillCool[uiIndex];
        Image WSkill = friendWSkillCool[uiIndex];

        healthBar.fillAmount = (float)character.status.CHP / character.status.MHP;
        QSkill.fillAmount = 1f - (character.skills[1].RemainCool / character.skills[1].coolDown);
        WSkill.fillAmount = 1f - (character.skills[2].RemainCool / character.skills[2].coolDown);

    }

    public void ShowSkillIndicator(int num)
    {
        curCharacter.ShowSkillIndicator(num, false);
    }

    public void HideSkillIndicator()
    {
        curCharacter.HideSkillIndicator();
    }

    public void MoveCamera(string direction)
    {
        Camera.main.GetComponent<CameraController>().MoveCamera(direction, true);
    }

    public void StopCamera(string direction)
    {
        Camera.main.GetComponent<CameraController>().MoveCamera(direction, false);
    }
}
