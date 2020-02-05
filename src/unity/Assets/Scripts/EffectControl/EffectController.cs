/*
 * 각 캐릭터에 부착되어 애니메이션, 사운드, 액션 특수효과 등을 제어하는 클래스 입니다.
 * 
 * 애니메이션 : 부착된 캐릭터의 State, 그리고 skillState에 따라 제어.
 * 
 * 원격캐릭터는 통신지연이 있으므로 rtt만큼 또 빠르게 보간해줘야 할것으로 보임.
 * 
 */ 



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EffectController : MonoBehaviour
{
    // 부착된 캐릭터의 애니메이터
    private Animator animator;

    // 캐릭터의 상태
    [SerializeField]
    private CharacterState characterState;

    // 캐릭터의 스킬들
    private readonly Skill[] skills = new Skill[3];

    // 캐릭터의 파티클
    public List<ParticleSystem> preParticles = new List<ParticleSystem>();
    public List<ParticleSystem> particles = new List<ParticleSystem>();

    // 상태 - 부울 딕셔너리
    [SerializeField]
    private Dictionary<string, bool> stateMap = new Dictionary<string, bool>();

    public GameObject[] projectilePrefabs = new GameObject[3];

    // 지속시간 있는 스킬이 발동중인지 알려주는 변수
    private bool[] isSkillActiveted = new bool[2];

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterState = GetComponent<Character>().GetCharacterState();
        for (int i = 0; i < 3; i++)
        {
            skills[i] = GetComponent<Character>().skills[i];
        }

        InitStateMap();

        Debug.Log("200130 : 테스트를 위해 딜러의 스킬1,2 의 선딜, 후딜을 1로 설정");
    }

    // Update is called once per frame
    void Update()
    {
        characterState = GetComponent<Character>().GetCharacterState();
        PlayAnimation();
        
    }

    // 상태맵 초기화
    private void InitStateMap()
    {
        stateMap.Add("Idle", false);
        stateMap.Add("Moving", false);
        //stateMap.Add("PreDelay", false);
        stateMap.Add("Fire", false);
        stateMap.Add("PostDelay", false);
        stateMap.Add("HardCC", false);
        stateMap.Add("Skill_0", false);
        stateMap.Add("Skill_1", false);
        stateMap.Add("Skill_2", false);
    }

    // 상태맵에서 원하는 상태만 True로 전환
    private void SetAnimStateMap(string stateName)
    {
        // Set True할 상태 먼저 Set.
        stateMap[stateName] = true;
        animator.SetBool(stateName, true);

        // 나머지는 False 처리
        foreach(var key in stateMap.Keys.ToList())
        {
            if(key != stateName)
            {
                stateMap[key] = false;
                animator.SetBool(key, false);
            }
        }
    }

    // Character / Skill state에 따라 맞는 애니메이션 재생
    // 분기별로 나누었으므로 해당 분기에서 사운드 등 적용 가능
    private void PlayAnimation()
    {
        // FixThis : 하드 CC 등 급박하게 애니메이션 바꿔야 하는 경우 탈출할 수 있도록 코드 작성 필요.
        /*
        if(characterState == CharacterState.HardCC)
        {
            
        }
        */
        // 씩씩이 테스트
        // Character의 상태가 HardCC가 되면, 애니메이션 재생. 
        // 상태가 다시 Idle로 바뀌게 되면 그 즉시 Idle 애니메이션 재생한다.
        if (animator.GetBool("CCTest"))
        {
            if (!stateMap["HardCC"])
            {
                SetAnimStateMap("HardCC");
            }
            animator.SetBool("CCTest", false);
            return;
        }

        // 사용한 스킬의 상태(idle~cooldown)에 따라 애니메이션 재생
        for (int i = 0; i < 3; i++)
        {
            if(skills[i].skillState == Skill.SkillState.Idle)
            {
                continue;
            }

            // 스킬의 상태에 맞는 애니메이션 설정 후 함수 종료.
            switch (skills[i].skillState)
            {
                case Skill.SkillState.PreDelay:
                    PlayPreDelayParticle(i);
                    PlayPreDelayAnim(i);
                    
                    return;
                case Skill.SkillState.Fire:
                    PlayFireParticle(i);
                    PlayFireAnim(i);
                    return;

                // 현재 Skill 에서 Fire 후 대기시간 없이 바로 PostDelay로 넘어가므로 Fire 애니메이션은 실행치 못한다.
                // 따라서 후딜 애니메이션은 따로 안넣고 Fire애니메이션으로 퉁치기 했음.
                // 후딜 시간도 후딜 애니메이션에 반영하려면 Fire에서 잠시 stop해야함.
                case Skill.SkillState.PostDelay:
                    PlayPostDelayAnim();
                    return;
            }      
        }

        // 캐릭터의 상태에 따라 애니메이션 재생
        if (characterState == CharacterState.Idle)
        { 
            PlayIdleAnim();
        }
        else if(characterState == CharacterState.Move)
        {
            PlayMovingAnim();
        }
    }

    // 스킬 선딜 애니메이션
    private void PlayPreDelayAnim(int index)
    {
        string skillName = "Skill_" + index.ToString();

        if (!stateMap[skillName])
        {
            SetAnimStateMap(skillName);
            animator.SetFloat("PreDelayOffset", 1f);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(skillName + "_PreDelay"))
        {
            Debug.Log("프리딜레이");
            // 선딜 시간에 맞춰 애니메이션 속도를 바꾸는데 필요없을거같은..?
            // SyncManager로부터 얻은 Rtt
            //float rtt = 0.4f;
            //float op = rtt > skills[index].preDelay ? rtt : skills[index].preDelay;
            // 1/offset은 애니메이션 재생 속도
            //float offset = op / animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + " , " + animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            //animator.SetFloat("PreDelayOffset", 1f / offset);
            if(skills[index].preDelay > 0f)
                animator.SetFloat("PreDelayOffset", 1f / (skills[index].preDelay / animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
        }        
    }
    // 스킬 발사 애니메이션
    private void PlayFireAnim(int index)
    {
        if (!stateMap["Fire"])
        {
            SetAnimStateMap("Fire");
            animator.SetFloat("PostDelayOffset", 1f);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Skill_" + index.ToString() + "_Fire"))
        {
            // PostDelay가 끝났을 때 애니메이션도 완전히 끝나도록 속도 조정한다.
            // Debug.Log(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length + ", " + skills[index].postDelay);

            // 클립이 없을경우, 딜레이가 0일경우(?) 예외처리 해야함. 나누기 0 
            if(skills[index].postDelay > 0f)
                animator.SetFloat("PostDelayOffset", 1f / (skills[index].postDelay / animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
            // animator.SetFloat("PostDelayOffset", 0.5f);
        }
    }

    // 스킬 후딜 애니메이션
    private void PlayPostDelayAnim()
    {
        if (!stateMap["PostDelay"])
        {
            SetAnimStateMap("PostDelay");
        }
    }

    // 이동 애니메이션
    private void PlayMovingAnim()
    {
        if (!stateMap["Moving"])
        {
            SetAnimStateMap("Moving");
        } 
    }

    // Idle 애니메이션
    private void PlayIdleAnim()
    {
        if (!stateMap["Idle"])
        {
            SetAnimStateMap("Idle");
        }
    }

    // 스킬 이펙트 파티클
    private void PlayPreDelayParticle(int skillNumber)
    {
        if (preParticles[skillNumber] == null)
            return;
        
        if (stateMap["Skill_" + skillNumber.ToString()])
            return;
        
        // 탱커
        if (GetComponent<Character>().index == 0)
        {
            if(skillNumber == 1)
            {
               
            }
            else if(skillNumber == 2)
            {

            }
        }
        // 딜러
        else if(GetComponent<Character>().index == 1)
        {
            if(skillNumber == 1)
            {
                ParticleSystem.MainModule psMain = preParticles[skillNumber].GetComponent<ParticleSystem>().main;
                psMain.startLifetime = skills[skillNumber].preDelay;
                preParticles[skillNumber].GetComponent<ParticleSystem>().Play();
            }
        }
        // 서포터
        else
        {
            if(skillNumber == 1)
            {
                StartCoroutine(PlayContinuousParticle(skillNumber));
            }
            else if(skillNumber == 2)
            {
                StartCoroutine(PlayContinuousParticle(skillNumber));
            }
        }
    }

    private void PlayFireParticle(int skillNumber)
    {
        if (particles[skillNumber] == null)
            return;
        
        if (stateMap["Fire"])
            return;

        // 0번째 캐릭터(탱커)
        if (GetComponent<Character>().index == 0)
        {
            if (particles[skillNumber].isPlaying)
                particles[skillNumber].Stop();

            ParticleSystem.MainModule psMain = particles[skillNumber].GetComponent<ParticleSystem>().main;
            
            particles[skillNumber].GetComponent<ParticleSystem>().Play();
        }
        // 1번째 캐릭터(딜러)
        else if (GetComponent<Character>().index == 1)
        {
            if (particles[skillNumber] == null) return;

            if (particles[skillNumber].isPlaying)
                particles[skillNumber].Stop();
            if (skillNumber == 1)
                particles[skillNumber].Play();
            else if (skillNumber == 2)
                StartCoroutine(PlayContinuousParticle(skillNumber));
        }
        // 2번째 캐릭터(서포터)
        else if(GetComponent<Character>().index == 2)
        {
            if (particles[skillNumber] == null) return;

            if (particles[skillNumber].isPlaying)
                particles[skillNumber].Stop();

            particles[skillNumber].GetComponent<ParticleSystem>().Play();
        }
    }


    // 지속시간 있는 스킬 처리
    private IEnumerator PlayContinuousParticle(int index)
    {
        // 딜러
        if(GetComponent<Character>().index == 1)
        {
            particles[index].GetComponent<ParticleSystem>().Play();

            // 치명타 스킬 발동 부울 변수 on
            isSkillActiveted[index - 1] = true;

            // 치명타 올리는 스킬 발동시 5초간의 지속시간을 가진다.
            if (index == 2)
                yield return new WaitForSeconds(5f);
            else
                yield return new WaitForSeconds(0.01f);

            particles[index].GetComponent<ParticleSystem>().Stop();
            
            // 치명타 스킬 발동 부울 변수 off
            isSkillActiveted[index - 1] = false;
        }
        // 서포터
        else if(GetComponent<Character>().index == 2)
        {
            // 광역 힐
            if(index == 1)
            {
                ParticleSystem.ShapeModule psShape = preParticles[index].GetComponent<ParticleSystem>().shape;
                psShape.radius = 1f;
                preParticles[index].Play();
                float tmp = 0f;
                while (tmp < skills[index].preDelay - .3f)
                {
                    psShape.radius += 0.05f;
                    yield return new WaitForSeconds(.9f);
                    tmp += 0.9f;
                }
                psShape.radius = 3f;
                yield return new WaitForSeconds(0.3f);
                preParticles[index].Stop();
            }
            // 상대에게 저주 발사
            else if(index == 2)
            {
                preParticles[index].Play();
                yield return new WaitForSeconds(skills[index].preDelay -.3f);
                preParticles[index].Stop();
            }
            
            
        }
    }

    // 스킬이 발동중 여부를 리턴하는 함수
    public bool SkillState(int index)
    {
        return isSkillActiveted[index - 1] ? true : false;
    }
}
