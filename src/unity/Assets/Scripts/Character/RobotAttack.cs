using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAttack
{
    public enum SkillState { Idle, Ready, PreDelay, Fire, PostDelay, CoolDown }

    [Header("Basic Info")]
    public SkillState skillState;

    [Header("Time")]
    public float preDelay;
    public float postDelay;
    public float coolDown;
    private float remainCool;
    public float RemainCool { get { return remainCool; } }

    [Header("Projectile Info")]
    public Projectile proj;
    public float speed;
    public float range;
    public Vector3 size;
    public TargetType targetType;
    public TargetNum targetNum;
    public List<SkillEffect> skillEffects;

    public IEnumerator Use(Robot caster, Vector3 dir)
    {
        #region Check Cool Time
        if (skillState != SkillState.Idle) yield break;
        #endregion

        skillState = SkillState.Ready;

        Debug.Log(caster.name + " Use to " + dir);

        // GM에게 index 번째 로봇이 dir 방향으로 공격한다고 알려준다.
        // GameManager.instance.FireProjectile(caster.index, dir);
    }

    public IEnumerator Fire(Character caster, Vector3 dir)
    {
        #region 선딜 대기

        // 스킬 발사 상태 변경 2020 01 30
        skillState = SkillState.PreDelay;

        // 20 02 09 내 캐릭터의 경우, preDelay + Rtt 만큼 더 기다림.
        yield return caster.is1P ? new WaitForSeconds(preDelay + SyncManager.instance.GetAvgRemoteRtt()) : new WaitForSeconds(preDelay);

        #endregion

        #region 투사체 발사

        // 20 02 09 체력이 0 이상이고, CC기 맞은 상태가 아닐 경우 투사체 발사.
        // cc기 피격 또는 사망시 코루틴을 아예 중지해 버리면 쿨타임 계산을 다시 해주어야 하므로 투사체만 발사하지 않는것으로 작성.
        // 상기 상황시 Character.cs 에서 character state 바뀌고, character state에 따라 EffectController.cs에서 사망/CC 애니메이션 재생함
        if (caster.GetCharacterState() != CharacterState.Die && caster.GetCharacterState() != CharacterState.CC)
        {
            // 스킬 발사 상태 변경 2020 01 30
            skillState = SkillState.Fire;

            ProjectileInfo info = ProjectileInfo(caster, dir);
            Vector3 spawnPos = caster.transform.position + new Vector3(0, 1.1f, 0);

            Projectile projectile = UnityEngine.Object.Instantiate(proj, spawnPos, Quaternion.identity);
            projectile.Initialize(info);
        }

        #endregion

        #region 후딜 대기
        yield return new WaitForSeconds(postDelay);
        #endregion

        skillState = SkillState.CoolDown;

        #region 쿨 돌리기
        remainCool = coolDown;

        while (remainCool > 0)
        {
            remainCool -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        remainCool = 0;
        #endregion

        skillState = SkillState.Idle;
    }

    public ProjectileInfo ProjectileInfo(Character caster, Vector3 dir)
    {
        ProjectileInfo info = new ProjectileInfo(caster, dir, speed, range, size, targetType, targetNum, skillEffects);
        return info;
    }

    public void Initialize()
    {
        skillState = SkillState.Idle;
        remainCool = 0f;
    }
}
