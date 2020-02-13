using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class Hero : MonoBehaviour
{
    public enum State { Idle, Appear, Skill, Disappear, CoolDown }

    [Header("Basic Info")]
    [SerializeField] private int index;
    [SerializeField] private bool is1P;
    [SerializeField] private State state;

    [Header("Skill Info")]
    [SerializeField] private Skill skill;

    public int Index { get { return index; } }
    public Skill GetSkill { get { return skill; } }

    public void UseSkill(Vector3 pos, Vector3? dir)
    {
        
    }

    private IEnumerator Fire()
    {
        if(state != State.Idle)
        {
            Debug.Log("영웅의 상태가 Idle이 아닙니다. 스킬 사용 명령을 무시합니다.");
            yield break;
        }

        #region 등장
        state = State.Appear;
        // 등장하는 애니메이션과 효과
        #endregion

        #region 스킬 사용
        state = State.Skill;
        // 스킬 사용
        // 투사체 생성
        #endregion

        #region 퇴장
        state = State.Disappear;
        // 퇴장하는 애니메이션과 효과
        #endregion

        #region 쿨타임
        state = State.CoolDown;
        yield return new WaitForSeconds(skill.coolDown);
        #endregion

        state = State.Idle;
    }

    private void Initialize(int num)
    {
        state = State.Idle;
        skill.Initialize();
    }
}
