using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum InputState { Normal, Action, Direction }

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private List<Character> characters;

    [Tooltip("초당 CP 증가량")]
    [SerializeField] private float cps;
    [SerializeField] private float myCP;

    [SerializeField] private InputState inputState = InputState.Normal;
    public InputState InputState { set { inputState = value; } }
    [SerializeField] private Character curCharacter;
    [SerializeField] private Character clickedCharacter;

    [Header("Display")]
    [SerializeField] private RectTransform moveCircle;
    
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].index = i;
        }

        myCP = 50;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCurrentCharacter(characters[0]);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCurrentCharacter(characters[1]);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCurrentCharacter(characters[2]);

        if (!curCharacter) return;

        if (Input.GetKeyDown(KeyCode.Z))
            Upgrade();
        if (Input.GetMouseButtonDown(1))
            ClickToMove();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("First Skill Input");
            curCharacter.UseSkill(0);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Second Skill Input");
            curCharacter.UseSkill(1);
        }
    }

    private void FixedUpdate()
    {
        // 매초 cps만큼 CP 충전
        myCP += cps * Time.fixedDeltaTime;
    }

    public Character ClickedCharacter()
    {
        switch (inputState)
        {
            case InputState.Normal:
                Debug.LogError("Normal 상태인데 Clicked Character 정보가 필요해?");
                return null;
            case InputState.Action:
                Character temp = clickedCharacter;
                clickedCharacter = null;
                return temp;
            default:
                return null;
        }
    }

    public void ChangeCurrentCharacter(Character character)
    {
        switch (inputState)
        {
            case InputState.Normal:
                if (curCharacter) curCharacter.ChooseToCurrent(false);
                curCharacter = character;
                curCharacter.ChooseToCurrent(true);
                break;
            case InputState.Action:
                clickedCharacter = character;
                break;
        }
    }

    public Vector3? GetDirection(Character caster, ref bool isValid)
    {
        Vector3? dir = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (curCharacter != caster)
        {
            isValid = false;
            return null;
        }

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 casterPos = caster.transform.position;
            Vector3 rawDir = hit.point - casterPos;
            caster.ShowSkillDirection(true, rawDir);

            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("마우스 위치 : " + hit.point + ", 시전자 위치 : " + casterPos + ", 계산된 방향 : " + rawDir);
                dir = rawDir.normalized;
                caster.ShowSkillDirection(false);
            }
        }

        return dir;
    }

    /// <summary>
    /// 선택된 캐릭터가 있고 마우스 오른쪽 클릭을 했을 때 해당 캐릭터의 목표지점을 설정
    /// </summary>
    private void ClickToMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100))
        {
            Animation anim = moveCircle.GetComponent<Animation>();
            anim.Stop();
            moveCircle.anchoredPosition = new Vector2(hit.point.x, hit.point.z);
            anim.Play();

            MoveCharacter(curCharacter.index, hit.point);
        }
    }

    /// <summary>
    /// characters[index]의 목표지점을 destination으로 설정해주고
    /// 이 정보를 서버로 전송
    /// </summary>
    /// <param name="index">캐릭터 번호</param>
    /// <param name="destination">목표 지점</param>
    public void MoveCharacter(int index, Vector3 destination)
    {
        characters[index].SetDestination(destination);
        MovingManager.instance.SendLocalMovingInfo(index, destination);
    }

    public void FireProjectile(int index, int num, Vector3 dir)
    {
        characters[index].FireProjectile(num, dir);
        SkillManager.instance.SendLocalSkillInfo(index, num, dir);
    }
    
    /// <summary>
    /// 임시로 만들어둔 캐릭터를 강화시키는 함수. 현재는 Z키를 누르면 CP를 20 소모하고 현재 선택된 캐릭터의 공격력이 20 증가.
    /// </summary>
    private void Upgrade()
    {
        if (myCP < 20) return;

        curCharacter.status.ChangeStat(StatusType.ATK, 20);
        myCP -= 20;
    }

    /// <summary>
    /// 계산된 효과를 target 캐릭터에게 적용하는 함수.
    /// 모든 공격, 효과는 이 함수를 거쳐가도록 할 예정.
    /// </summary>
    /// <param name="target">효과를 적용할 대상</param>
    /// <param name="effects">적용할 효과의 리스트</param>
    public void ApplySkill(Character target, List<EffectResult> effects)
    {
        target.Apply(effects);
    }
}