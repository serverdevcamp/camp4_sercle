using System.Collections;
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

    private Character curCharacter;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        DisplayCurrentCharacterInfo();
        DisplayFriendCharacterInfo();
    }

    /// <summary>
    /// 현재 캐릭터의 정보를 나타내는 함수.
    /// 체력바, 스탯 정보, 스킬 쿨타임 표시.
    /// </summary>
    private void DisplayCurrentCharacterInfo()
    {
        curCharacter = GameManager.instance.CurCharacter;
        if (curCharacter == null) return;

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

    private void DisplayFriendCharacterInfo()
    {

    }
}
