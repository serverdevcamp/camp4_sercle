using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class IndicateManager : MonoBehaviour
{
    public enum State { None, Position, Direction }
    public SplatManager Splats { get; set; }

    private State state;
    private Hero nowHero;

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
                nowHero = null;
                Splats.CancelSpellIndicator();
                Splats.CancelRangeIndicator();
                break;
            case State.Position:
                Skill skill = nowHero.GetSkill;

                transform.position = Splats.Get3DMousePosition();
                if (skill.range != 0) Splats.SelectSpellIndicator("Direction");
                else Splats.SelectRangeIndicator("Range");
                // 스킬의 위치를 지정받기 위해 인디케이터를 키고 마우스 down을 기다립니다.
                if (Input.GetMouseButtonDown(0))
                {
                    if (skill.range != 0)
                    {
                        state = State.Direction;
                    }
                    else
                    {
                        Debug.Log(skill.skillName + "을 " + transform.position + "에 사용합니다.");
                        // gm에게 index번의 영웅의 스킬을 skillPos에 사용한다고 알립니다.
                        GameManager.instance.RequestFire(1, false, nowHero.Index, transform.position, Vector3.zero);
                        state = State.None;
                    }
                }
                break;
            case State.Direction:
                // 스킬의 방향을 입력받기 위해 마우스 up을 기다립니다.
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 dir = Splats.Get3DMousePosition() - transform.position;
                    Debug.Log(nowHero.GetSkill.skillName + "을 " + transform.position + "에 " + dir +" 방향으로 사용합니다.");
                    // gm에게 index번의 영웅의 스킬을 skillPos에 dir 방향으로 사용한다고 알립니다.
                    GameManager.instance.RequestFire(1, false, nowHero.Index, transform.position, dir);
                    state = State.None;
                }
                break;
        }

        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
    }

    public void ActivateSkillIndicator(Hero hero)
    {
        Splats.CancelSpellIndicator();
        Splats.CancelRangeIndicator();

        // 인디케이터의 상태를 skill에 알맞은 상태로 바꿉니다.
        nowHero = hero;
        Splats.GetRangeIndicator("Range").DefaultScale = nowHero.GetSkill.size;
        state = State.Position;
    }
}
