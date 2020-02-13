using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class IndicateManager : MonoBehaviour
{
    public enum State { None, Position, Direction }
    public SplatManager Splats { get; set; }

    private State state;
    private Skill nowSkill;

    private void Start()
    {
        Splats = GetComponent<SplatManager>();
        state = State.None;
    }

    private void Update()
    {
        StateAction();
    }

    private void StateAction()
    {
        switch (state)
        {
            case State.None:
                // 전부 원래 상태로 돌립니다!
                transform.position = Vector3.zero;
                nowSkill = null;
                Splats.CancelSpellIndicator();
                Splats.CancelRangeIndicator();
                break;
            case State.Position:
                transform.position = Splats.Get3DMousePosition();
                if (nowSkill.range != 0) Splats.SelectSpellIndicator("Direction");
                else Splats.SelectRangeIndicator("Range");
                // 스킬의 위치를 지정받기 위해 인디케이터를 키고 마우스 down을 기다립니다.
                if (Input.GetMouseButtonDown(0))
                {
                    if (nowSkill.range != 0)
                    {
                        state = State.Direction;
                    }
                    else
                    {
                        Debug.Log(nowSkill.skillName + "을 " + transform.position + "에 사용합니다.");
                        state = State.None;
                        // gm에게 index번의 영웅의 스킬을 skillPos에 사용한다고 알립니다.
                    }
                }
                break;
            case State.Direction:
                // 스킬의 방향을 입력받기 위해 마우스 up을 기다립니다.
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 dir = Splats.Get3DMousePosition() - transform.position;
                    // gm에게 index번의 영웅의 스킬을 skillPos에 dir 방향으로 사용한다고 알립니다.

                    state = State.None;
                }
                break;
        }
    }

    public void ActivateSkillIndicator(Skill skill)
    {
        // 인디케이터의 상태를 skill에 알맞은 상태로 바꿉니다.
        nowSkill = skill;
        state = State.Position;
    }
}
