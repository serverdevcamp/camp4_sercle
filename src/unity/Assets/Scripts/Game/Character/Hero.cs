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
    [SerializeField] private GameObject skillEffect;

    [Header("Animation")]
    [SerializeField] private Animator heroAnim;

    [Header("Miscellaneous Effect")]
    [SerializeField] private GameObject exitEffect;


    private void Start()
    {
        heroAnim = GetComponentInChildren<Animator>();
    }

    public int Index { get { return index; } }
    public Skill GetSkill { get { return skill; } }

    public void UseSkill(Vector3 pos, Vector3? dir)
    {

        StartCoroutine(Fire(pos, dir));

    }

    private IEnumerator Fire(Vector3 pos, Vector3? dir)
    {
        if(state != State.Idle)
        {
            Debug.Log("영웅의 상태가 Idle이 아닙니다. 스킬 사용 명령을 무시합니다.");
            yield break;
        }

        #region 등장
        state = State.Appear;
        // 등장하는 애니메이션과 효과

        // 20 02 14 영웅의 위치를 pos + 10로 이동
        GetComponent<Transform>().position = pos + new Vector3(0, 10, 0);
        heroAnim.SetTrigger("Emerge");

        // 하늘에서 강하.
        Rigidbody rig = GetComponent<Rigidbody>();
        if (rig)
        {
            rig.AddForce(-this.transform.up * 300f, ForceMode.Impulse);
        }

        // 착지 후 멋져보이게 0.5초 대기
        yield return new WaitForSeconds(.5f);
        #endregion

        #region 스킬 사용
        //yield return new WaitForSeconds(skill.preDelay);
        //heroAnim.SetTrigger("Fire");
        // 스킬 사용

        // 투사체 생성
        state = State.Skill;
        if (dir.HasValue)
            Instantiate(skillEffect, pos, Quaternion.Euler(dir.Value));
        else
            Instantiate(skillEffect, pos, Quaternion.identity);
        #endregion

        #region 퇴장
        state = State.Disappear;
        // 퇴장하는 애니메이션과 효과
        Instantiate(exitEffect, pos, Quaternion.Euler(-90, 0,0));
        yield return new WaitForSeconds(skill.postDelay);
        transform.position = new Vector3(0999, 0999, 9990);

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
