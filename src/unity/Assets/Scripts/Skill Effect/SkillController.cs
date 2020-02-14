/*
 * 스킬 발동을 담당하는 컨트롤러 입니다.
 * 
 * 매칭 후 스킬 선택 씬에서, 플레이어는 스킬 3가지? 를 선택한다.
 * 스킬선택 씬에서 게임 씬으로 넘어올 때, switch문으로 skillType으로 조건에 맞다면 이 스크립트의 localSkills에 스킬 프리팹을 등록한다.
 * 게임씬으로 넘어올 때, 상대가 1번~3번까지 어떤 스킬을 선택했는지 remoteSkills에 스킬 프리팹을 등록한다.
 * 
 * 이제 SkillManager.cs에서 상대 스킬 정보를 줄 때, 스킬 번호와(1~3) 좌표만 주면 됨.
 * 
 * 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{

    // 임시 스킬 
    public enum TempSkillType { DarkAttack, FireAttack, HealBuff, DarkDebuff }
    public enum TempSkillCaster { Local, Remote }
    public class TempSkill
    {
        TempSkillType type;
        float amount;
        // + skillEffects 들어감

        public TempSkill(TempSkillType initType, float initAmount) { type = initType; amount = initAmount; }
    }

    public int curChoosedSkillNumber;
    public Quaternion curRotation;

    // 이전 씬에서 (나) 플레이어가 선택한 스킬들 
    public GameObject[] localSkills = new GameObject[3];
    // 이전 씬에서 상대 플레이어가 선택한 스킬들
    public GameObject[] remoteSkills = new GameObject[3];

    public TempSkill[] skill = new TempSkill[3];

    public GameObject me;
    public GameObject spawnEffect;

    // Start is called before the first frame update
    void Start()
    {
        // InitSkillInfo();

        curChoosedSkillNumber = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        ChoiceSkill();
        DetectSkillPositionAndActivateSkill();
    }

    // 스킬 선택 씬에서 선택된 스킬들은 array에 등록되어 있으므로 그 스킬들을 참조하여 정보 초기화.
    private void InitSkillInfo()
    {
        for(int i = 0; i < 3; i++)
        {
            // 일단 로컬(내) 스킬들 정보만 초기화했음.
            
            // 스킬 프리팹에 따로 스크립트 부착(TempSkillType, Amount, etc,. 용도)
            // skill[i] = new TempSkill(localSkills[i].skillType, localSkills[i].amount);
            if(i == 0)
            {
                skill[i] = new TempSkill(TempSkillType.DarkAttack, 30f);
            }
            else if (i == 1)
            {
                skill[i] = new TempSkill(TempSkillType.HealBuff, 20f);
            }
            else if (i == 2)
            {
                skill[i] = new TempSkill(TempSkillType.DarkDebuff, 10f);
            }
            
            // 리모트 스킬 초기화
            
        }
    }


    // 위치 받고 거기에 스킬 발동. 나중에 함수 2개로 쪼개기
    // 실제로는, 로컬 -> 서버로 스킬 요청
    // 서버 -> 각 단말로 송신
    // recv 하면 수신한 위치, 타입으로 스킬 생성.
    private void DetectSkillPositionAndActivateSkill()
    {
        if (curChoosedSkillNumber == -1) return;

        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            StartCoroutine(EmergeCharacterAndActivateSkill(curChoosedSkillNumber, hit.point));

            curChoosedSkillNumber = -1;
        }
    }

    // demi god emerged
    private IEnumerator EmergeCharacterAndActivateSkill(int skillNum, Vector3 point)
    {
        GameObject spawn = Instantiate(spawnEffect, point, Quaternion.identity);

        yield return new WaitForSeconds(.5f);

        GameObject tmpMe = Instantiate(me, point, curRotation);

        Destroy(spawn);

        // 선딜
        yield return new WaitForSeconds(1.5f);

        Instantiate(localSkills[skillNum], point, curRotation);
        Debug.Log(skillNum + " " + localSkills[skillNum].name + " " + point + " 발동.");
        Instantiate(spawnEffect, point, Quaternion.identity);
        // 후딜
        yield return new WaitForSeconds(1f);

        Destroy(tmpMe);
    }

    public void ChoiceSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (curChoosedSkillNumber == 0) curRotation = Quaternion.Euler(0, 90, 0);
            else curRotation = Quaternion.Euler(0, 0, 0);

            curChoosedSkillNumber = 0;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (curChoosedSkillNumber == 1) curRotation = Quaternion.Euler(0, 90, 0);
            else curRotation = Quaternion.Euler(0, 0, 0);

            curChoosedSkillNumber = 1;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (curChoosedSkillNumber == 2) curRotation = Quaternion.Euler(0, 90, 0);
            else curRotation = Quaternion.Euler(0, 0, 0);

            curChoosedSkillNumber = 2;
        }
    }


}
