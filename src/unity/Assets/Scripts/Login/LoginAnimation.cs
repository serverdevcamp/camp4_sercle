using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoginAnimation : MonoBehaviour
{
    // 교체할 캐릭터 프리팹
    public List<GameObject> characterPrefabs;

    // FSM에 사용될 열거형
    private enum AnimationState { None, WalkToStage, Idle, Skill, WalkToBackStage }

    // 현재 조종되는 캐릭터
    private GameObject character;

    // 현재 조종되는 캐릭터의 상태
    [SerializeField]
    private AnimationState state;
    [SerializeField]
    private int nextCharacterPtr;

    // stage 위치
    public Vector3 stagePos;
    // back stage 위치
    public Vector3 backStagePos;

    // Start is called before the first frame update
    void Start()
    {
        state = AnimationState.None;
        if (characterPrefabs.Count >= 0)
            character = characterPrefabs[0];
        StartCoroutine(AnimationFSM());
    }


    // 로그인 애니메이션 유한상태머신
    private IEnumerator AnimationFSM()
    {
        yield return new WaitForSeconds(1f);

        nextCharacterPtr = -1;

        while (true)
        {
            switch (state)
            {
                case AnimationState.None:
                    ChangeCharacter();
                    ChangeState(AnimationState.WalkToStage);
                    break;
                case AnimationState.WalkToStage:
                    DoWalkToStage();
                    break;
                case AnimationState.Idle:
                    DoIdle();
                    break;
                case AnimationState.Skill:
                    DoSkill();
                    break;
                case AnimationState.WalkToBackStage:
                    DoWalkToBackStage();
                    break;
                default:
                    Debug.LogError("No such Animation State.");
                    break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // 현재 애니메이션 상태를 변화시키는 함수.
    private void ChangeState(AnimationState st)
    {
        state = st;
    }

    // 다음 캐릭터 게임오브젝트를 선정.
    private void ChangeCharacter()
    {
        nextCharacterPtr = ++nextCharacterPtr % 3;
        character = characterPrefabs[nextCharacterPtr];
    }

    // 스테이지로 걸어가는 상태
    private void DoWalkToStage()
    {
        if (Vector3.Distance(character.transform.position, stagePos) <= 0.05f)
        {
            ChangeState(AnimationState.Idle);
            return;
        }

        character.transform.DOMove(stagePos, 3f, false);     
    }

    // 백 스테이지로 걸어가는 상태
    private void DoWalkToBackStage()
    {
        if (Vector3.Distance(character.transform.position, backStagePos) <= 0.05f)
        {
            ChangeState(AnimationState.None);
            return;
        }

        character.transform.DOMove(backStagePos, 3f, false);
    }

    // 스테이지에서 Idle 애니메이션 재생하는 상태
    private void DoIdle()
    {
        if (!character.GetComponent<Animator>().GetBool("Idle"))
        {
            character.GetComponent<Animator>().SetBool("Idle", true);
            StartCoroutine(WaitForNextState());   
        }
    }

    // 현재 상태에 따라 다음 상태로 가는데 필요한 대기시간만큼 대기하는 코루틴
    private IEnumerator WaitForNextState()
    {
        switch (state)
        {
            case AnimationState.Idle:
                yield return new WaitForSeconds(10f);
                ChangeState(AnimationState.Skill);
                character.GetComponent<Animator>().SetBool("Idle", false);
                break;

            case AnimationState.Skill:
                yield return new WaitForSeconds(7f);
                ChangeState(AnimationState.WalkToBackStage);
                character.GetComponent<Animator>().SetBool("Skill", false);
                break;

            default:
                yield return new WaitForSeconds(0f);
                break;
        }
    }

    // 스테이지에서 스킬 애니메이션 재생하는 상태
    private void DoSkill()
    {
        if (!character.GetComponent<Animator>().GetBool("Skill"))
        {
            character.GetComponent<Animator>().SetBool("Skill", true);
            StartCoroutine(WaitForNextState());
        }
    }
}
