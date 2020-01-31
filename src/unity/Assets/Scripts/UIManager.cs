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
        DisplayCurrentCharacterStatus();
    }

    private void DisplayCurrentCharacterStatus()
    {
        curCharacter = GameManager.instance.CurCharacter;
        if (curCharacter == null) return;

        // health bar
        curHealthBar.fillAmount = (float)curCharacter.status.CHP / curCharacter.status.MHP;
        // status
        for (int i = 0; i < statusText.Count; i++)
        {
            statusText[i].text = curCharacter.status.Value((StatusType)(i + 3)).ToString();
        }
        // skill cool time
        for (int i = 0; i < skillCool.Count; i++)
        {
            Skill skill = curCharacter.skills[i];
            Image image = skillCool[i];

            image.fillAmount = skill.RemaingCool / skill.coolDown;
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
}
