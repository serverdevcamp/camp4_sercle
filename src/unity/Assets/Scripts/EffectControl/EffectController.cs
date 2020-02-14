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
using DG.Tweening;

public class EffectController : MonoBehaviour
{
    // 부착된 캐릭터의 애니메이터
    private Animator animator;

    // 캐릭터의 상태
    [SerializeField]
    private CharacterState characterState;

    // 캐릭터의 스킬들
    private readonly Skill[] skills = new Skill[3];

    [Header("Skill Particles")]
    // 캐릭터의 파티클
    public List<ParticleSystem> preParticles = new List<ParticleSystem>();
    public List<ParticleSystem> particles = new List<ParticleSystem>();

    // 탱커의 검격
    public TrailRenderer swordTrail;

    // 상태 - 부울 딕셔너리
    [SerializeField]
    private Dictionary<string, bool> stateMap = new Dictionary<string, bool>();

    public GameObject[] projectilePrefabs = new GameObject[3];

    // 지속시간 있는 스킬이 발동중인지 알려주는 변수
    private bool[] isSkillActiveted = new bool[2];

    [Header("Hit Particles")]
    // 힐 피격시 나타낼 파티클
    public ParticleSystem[] healEffects;

    // 스턴 피격시 나타낼 파티클
    public ParticleSystem stunEffect;

    // 일반 공격 피격시 나타낼 파티클
    public ParticleSystem normalHitEffect;

    // 자기 버프 사용시 나타낼 파티클
    public ParticleSystem buffEffect;

    // 디버프 피격시 나타낼 파티클
    public ParticleSystem debuffEffect;

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
        stateMap.Add("Fire", false);
        stateMap.Add("PostDelay", false);
        stateMap.Add("HardCC", false);
        stateMap.Add("Skill_0", false);
        stateMap.Add("Skill_1", false);
        stateMap.Add("Skill_2", false);
        stateMap.Add("Die", false);
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
        // 사망 애니메이션 재생
        if(characterState == CharacterState.Die)
        {
            PlayDieAnim();
            return;
        }

        // CC 애니메이션 재생
        if(characterState == CharacterState.CC)
        {
            PlayCCAnim();
            return;
        }

        // 사용한 스킬의 상태(idle~cooldown)에 따라 애니메이션 재생
        for (int i = 0; i < 3; i++)
        {
            if(skills[i].state == Skill.State.Idle)
            {
                continue;
            }

            // 스킬의 상태에 맞는 애니메이션 설정 후 함수 종료.
            switch (skills[i].state)
            {
                case Skill.State.PreDelay:
                    PlayPreDelayParticle(i);
                    PlayPreDelayAnim(i);
                    
                    return;
                case Skill.State.Fire:
                    PlayFireParticle(i);
                    OnFireVFX(i);
                    PlayFireAnim(i);
                    return;

                case Skill.State.PostDelay:
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

            if(GetComponent<Character>().index == 0 && index == 0)
            {
                TankerSwordTrail();
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(skillName + "_PreDelay"))
        {
            // PreDelay 시간에 맞추어 애니메이션 재생속도를 조정한다.
            if (skills[index].preDelay > 0f)
                animator.SetFloat("PreDelayOffset", 1f / ((GetComponent<Character>().isFriend ? skills[index].preDelay + SyncManager.instance.GetAvgRemoteRtt() : skills[index].preDelay) / animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
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

    // Die 애니메이션
    private void PlayDieAnim()
    {
        if (!stateMap["Die"])
        {
            SetAnimStateMap("Die");
            this.enabled = false;
        }
    }

    // CC 피격 애니메이션
    private void PlayCCAnim()
    {
        if (!stateMap["HardCC"])
        {
            SetAnimStateMap("HardCC");
        }
    }

    // 탱커 검격 발동
    private void TankerSwordTrail()
    {
        StartCoroutine(PlayContinuousParticle(0));
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
            if (skillNumber == 0)
            {
                
            }
            else if(skillNumber == 1)
            {
                preParticles[skillNumber].Play();
            }
            else if(skillNumber == 2)
            {
                ParticleSystem.MainModule psMain = preParticles[skillNumber].GetComponent<ParticleSystem>().main;
                Debug.Log("탱커 2스킬 radius 5로 하드코딩");
                psMain.startSize = 5f;
                preParticles[skillNumber].Play();
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

    // 투사체에 히트 당했을 때 VFX
    public void OnHitVFX(SkillEffect effect, int casterIndex)
    {
        switch (effect.GetSkillType())
        {
            case SkillEffect.Type.Heal:
                for (int i = 0; i < healEffects.Length; i++)
                {
                    healEffects[i].Play();
                }
                break;
            case SkillEffect.Type.Attack:
                normalHitEffect.Play();
                break;
            case SkillEffect.Type.Buff:
                buffEffect.Play();
                break;
            case SkillEffect.Type.Debuff:
                debuffEffect.Play();
                break;
            case SkillEffect.Type.Stun:
                stunEffect.Play();
                break;
        }
    }

    // 스킬이 Fire 상태일 때 VFX
    private void OnFireVFX(int index)
    {
        if (stateMap["Fire"])
            return;

        // 아군일 경우
        if (GetComponent<Character>().isFriend)
        {
            // 탱/ 딜/ 힐
            switch (GetComponent<Character>().index)
            {
                // 탱커
                case 0:
                    switch (index)
                    {
                        case 0:
                            break;
                        case 1:
                            // 탱커 1번째 스킬의 경우, 잔상 남도록 설정.
                            StartCoroutine(PlayContinuousParticle(index));
                            break;
                        case 2:
                            // 탱커의 2번째 스킬의 경우, 카메라 흔들기 효과 
                            Camera.main.transform.DOShakePosition(0.1f, 0.3f, 5);
                          
                            break;
                    }
                    break;
                // 딜러
                case 1:
                    switch (index)
                    {
                        case 0:
                            break;
                        case 1:
                            // 딜러의 1번째 스킬의 경우, 카메라 흔들기 효과 
                            Camera.main.transform.DOShakePosition(0.1f, 0.1f, 1);
                            break;
                        case 2:
                            break;
                    }
                    break;
                // 힐러
                case 2:
                    break;
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
            {
                particles[skillNumber].Play();
            }
            else if (skillNumber == 2)
                StartCoroutine(PlayContinuousParticle(skillNumber));
        }
        // 2번째 캐릭터(서포터)
        else if (GetComponent<Character>().index == 2)
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
        // 탱커
        if(GetComponent<Character>().index == 0)
        {
            if(index == 0)
            {
                swordTrail.enabled = true;
                yield return new WaitForSeconds(0.5f);
                swordTrail.enabled = false;
            }
            else if(index == 1)
            {
                GetComponent<TrailRenderer>().enabled = true;
                yield return new WaitForSeconds(.5f);
                GetComponent<TrailRenderer>().enabled = false;
            }
        }
        // 딜러
        else if(GetComponent<Character>().index == 1)
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
