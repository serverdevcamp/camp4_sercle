using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public enum State { Idle, Ready, PreDelay, Fire, PostDelay, CoolDown }

    [Header("Basic Info")]
    public State state;
    public SkillEffect effect;

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
    public float size;

    public IEnumerator Use(Robot caster, Vector3 dir)
    {
        #region Check Cool Time
        if (state != State.Idle) yield break;
        #endregion

        state = State.Ready;

        Debug.Log(caster.name + " use basic attack to " + dir);

        // GM에게 index 번째 로봇의 공격을 dir 방향으로 사용한다고 알려준다.
        GameManager.instance.RequestFire(caster.CampNum, true, caster.Index, caster.transform.position, dir);
    }

    public IEnumerator Fire(Robot caster, Vector3 dir, bool isReallyFire)
    {
        #region Wait for Pre delay
        state = State.PreDelay;

        // 20 02 09 내 캐릭터의 경우, preDelay + Rtt 만큼 더 기다림.
        yield return new WaitForSeconds(preDelay);
        #endregion

        if (isReallyFire)
        {
            #region 투사체 발사

            // 20 02 09 체력이 0 이상이고, CC기 맞은 상태가 아닐 경우 투사체 발사.
            // cc기 피격 또는 사망시 코루틴을 아예 중지해 버리면 쿨타임 계산을 다시 해주어야 하므로 투사체만 발사하지 않는것으로 작성.
            // 상기 상황시 Character.cs 에서 character state 바뀌고, character state에 따라 EffectController.cs에서 사망/CC 애니메이션 재생함
            if (caster.GetState != Robot.State.Die && caster.GetState != Robot.State.CC)
            {
                // 스킬 발사 상태 변경 2020 01 30
                state = State.Fire;

                ProjectileInfo info = ProjectileInfo(caster, dir);
                Vector3 spawnPos = caster.transform.position + new Vector3(0, 1.1f, 0);

                Projectile projectile = UnityEngine.Object.Instantiate(proj, spawnPos, Quaternion.identity);
                projectile.Initialize(info);

                caster.ShowMuzzleEffect(true);
                SoundManager.instance.PlaySound("RobotAttack", 0.1f);
            }
            #endregion
        }

        #region Wait for Post Delay
        yield return new WaitForSeconds(postDelay);
        caster.ShowMuzzleEffect(false);
        #endregion

        state = State.CoolDown;

        #region Cool Down...
        remainCool = coolDown;

        while (remainCool > 0)
        {
            remainCool -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        remainCool = 0;
        #endregion

        state = State.Idle;
    }

    private ProjectileInfo ProjectileInfo(Robot caster, Vector3 dir)
    {
        List<SkillEffect> effects = new List<SkillEffect>();
        effect.amount = -(int)caster.GetStatus.ATK;
        effects.Add(effect);

        ProjectileInfo info = new ProjectileInfo(caster.CampNum, dir, speed, range, size, TargetType.Enemy, TargetNum.One, effects);
        return info;
    }
}