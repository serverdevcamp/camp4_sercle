using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Characters")]
    public List<GameObject> characterPrefabs;
    public List<Character> myCharacters;
    public List<Character> enemyCharacters;
    public List<Vector3> startPos_1P;
    public List<Vector3> startPos_2P;

    [SerializeField] private Character curCharacter;
    public Character CurCharacter { get { return curCharacter; } }

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
        bool is1P;
        if(GameObject.Find("UserInfoObject").GetComponent<UserInfo>().userData.playerCamp == 1)
        {
            is1P = true;
        }
        else
        {
            is1P = false;
        }

        #region 캐릭터 생성 및 번호/ID 부여
        // 현재는 캐릭터가 3개 밖에 없으므로 자동으로 생성
        // 나중에는 서버에서 받아와야 함
        myCharacters = new List<Character>();
        for (int i = 0; i < 3; i++)
        {
            Character _character = Instantiate(characterPrefabs[i]).GetComponent<Character>();
            _character.name = characterPrefabs[i].name;
            _character.index = i;
            _character.isFriend = true;

            myCharacters.Add(_character); 
        }

        enemyCharacters = new List<Character>();
        for (int i = 0; i < 3; i++)
        {
            Character _character = Instantiate(characterPrefabs[i]).GetComponent<Character>();
            _character.name = "Enemy_" + characterPrefabs[i].name;
            _character.index = i;
            _character.isFriend = false;

            enemyCharacters.Add(_character);
        }
        #endregion

        #region 캐릭터 배치
        // 회전도 해줘야 할까?
        if (is1P)
        {
            for (int i = 0; i < 3; i++)
            {
                myCharacters[i].GetComponent<NavMeshAgent>().enabled = false;
                enemyCharacters[i].GetComponent<NavMeshAgent>().enabled = false;
                myCharacters[i].transform.position = startPos_1P[i];
                enemyCharacters[i].transform.position = startPos_2P[i];
                myCharacters[i].GetComponent<NavMeshAgent>().enabled = true;
                enemyCharacters[i].GetComponent<NavMeshAgent>().enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                myCharacters[i].GetComponent<NavMeshAgent>().enabled = false;
                enemyCharacters[i].GetComponent<NavMeshAgent>().enabled = false;
                myCharacters[i].transform.position = startPos_2P[i];
                enemyCharacters[i].transform.position = startPos_1P[i];
                myCharacters[i].GetComponent<NavMeshAgent>().enabled = true;
                enemyCharacters[i].GetComponent<NavMeshAgent>().enabled = true;
            }
        }
        #endregion

        #region 시작 세팅
        // 자신의 첫번째 캐릭터로 고정
        ChangeCurrentCharacter(myCharacters[0]);
        #endregion
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeCurrentCharacter(myCharacters[0]);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeCurrentCharacter(myCharacters[1]);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeCurrentCharacter(myCharacters[2]);

        if (!curCharacter) return;

        // 20 02 10 사망한 캐릭터/ hard cc 상황인 캐릭터로는 조작이 불가하도록 함.
        if (curCharacter.GetCharacterState() == CharacterState.Die || curCharacter.GetCharacterState() == CharacterState.CC) return;

        if (Input.GetMouseButtonDown(1))
            ClickToMove();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("First Skill Input");
            curCharacter.UseSkill(1);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Second Skill Input");
            curCharacter.UseSkill(2);
        }
    }

    public void ChangeCurrentCharacter(Character character)
    {
        if (curCharacter) curCharacter.ChooseToCurrent(false);
        curCharacter = character;
        Camera.main.GetComponent<CameraController>().FocusCharacter(CurCharacter);
        curCharacter.ChooseToCurrent(true);
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

            if (Input.GetMouseButtonDown(0))
            {
                // Debug.Log("마우스 위치 : " + hit.point + ", 시전자 위치 : " + casterPos + ", 계산된 방향 : " + rawDir);
                dir = rawDir.normalized;
                //caster.ShowSkillDirection(false);
            }
        }

        return dir;
    }

    /// <summary>
    /// 선택된 캐릭터가 있고 마우스 오른쪽 클릭을 했을 때 해당 캐릭터의 목표지점을 설정
    /// </summary>
    private void ClickToMove()
    {
        // 20 02 11 현재 캐릭터가 스킬 사용중이라면 이동 불가
        if (curCharacter.usingSkill) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 100))
        {
            Animation anim = moveCircle.GetComponent<Animation>();
            anim.Stop();
            moveCircle.anchoredPosition = new Vector2(hit.point.x, hit.point.z);
            anim.Play();

            for(int i = 0; i < 3; i++)
            {
                if(curCharacter.index == i) MoveCharacter(i, hit.point);
                else MoveCharacter(i, hit.point, false);
            }
        }
    }

    /// <summary>
    /// characters[index]의 목표지점을 destination으로 설정해주고
    /// 이 정보를 서버로 전송
    /// </summary>
    /// <param name="index">캐릭터 번호</param>
    /// <param name="destination">목표 지점</param>
    /// <param name="isCurrent">Current Character일 때만 true로 설정</param>
    public void MoveCharacter(int index, Vector3 destination, bool isCurrent = true)
    {
        myCharacters[index].SetDestination(destination, isCurrent);
        MovingManager.instance.SendLocalMovingInfo(index, destination);
    }

    // 2020 02 01 수신한 원격 캐릭터 이동 패킷을 원격캐릭터에 적용 한다.
    public void MoveEnemyCharacter(int index, Vector3 destination)
    {
        enemyCharacters[index].SetDestination(destination);
    }

    // 2020 02 07 목적지와 현 위치의 거리를 보정한 원격 캐릭터의 속도 계산.
    public float SetInterpolatedSpeed(int index, Vector3 destination)
    {
        float prevTime = Vector3.Distance(enemyCharacters[index].GetComponent<Transform>().position, destination) / enemyCharacters[index].status.SPD;

        return (enemyCharacters[index].status.SPD) * (prevTime + SyncManager.instance.GetAvgRemoteRtt()) / prevTime;
    }

    public void FireProjectile(int index, int num, Vector3 dir)
    {
        myCharacters[index].FireProjectile(num, dir);
        SkillManager.instance.SendLocalSkillInfo(index, num, dir);
    }

    // 2020 02 01 원격 캐릭터가 투사체 발사하도록 한다.
    public void FireRemoteProjectile(int index, int num, Vector3 dir)
    {
        enemyCharacters[index].FireProjectile(num, dir);
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
