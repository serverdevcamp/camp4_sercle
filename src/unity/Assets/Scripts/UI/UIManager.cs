using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject skillPanel;

    [Header("Skill Input Profile")]
    [SerializeField] private Animator skill_Base;
    [SerializeField] private Animator skill_Q;
    [SerializeField] private Animator skill_W;

    private List<GameObject> skillButtons = new List<GameObject>();
    private int heroCount;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        heroCount = GameManager.instance.GetMyHeroCount();

        for (int i = 0; i < heroCount; i++)
        {
            GameObject skillButton = Instantiate(buttonPrefab, skillPanel.transform);
            skillButton.GetComponent<SkillButtonIndicator>().Initialize(GameManager.instance.GetMyHero(i));
            skillButtons.Add(skillButton);
        }

        AlignSkillButtons();
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

    private void AlignSkillButtons()
    {
        float buttonSizeX = 200;
        float buttonSizeY = 200;

        for (int i = 0; i < skillButtons.Count; i++)
        {
            GetComponent<RectTransform>().anchoredPosition += new Vector2(i * buttonSizeX, 0);
        }
    }
}
