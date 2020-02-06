using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public enum RangeType { Self, Around, Direction }
public enum TargetType { Auto, Self, Friend, Enemy }
public enum TargetNum { One, All }

[System.Serializable]
public class Skill
{
    public enum SkillState { Idle, Ready, PreDelay, Fire, PostDelay, CoolDown }

    [Header("Basic Info")]
    public string skillName;
    public string description;
    public SkillState skillState;
    public Splat indicator;
    public int myNum;

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

    public IEnumerator Use(Character caster, Vector3? dir = null)
    {

        #region Check Cool Time
        if (skillState != SkillState.Idle) yield break;
        #endregion

        skillState = SkillState.Ready;

        #region Get Direction
        if (speed == 0)
        {
            dir = Vector3.zero;
        }
        else
        {
            caster.ShowSkillIndicator(myNum, true);
            while (dir.HasValue == false)
            {
                bool isValid = true;
                dir = GameManager.instance.GetDirection(caster, ref isValid);

                if (isValid == false) yield break;

                yield return new WaitForFixedUpdate();
            }
            caster.HideSkillIndicator();
        }
        #endregion

        Debug.Log(caster.name + " Use " + skillName + " to " + dir);

        // GM에게 index 번째 캐릭터의 num번째 스킬을 dir 방향으로 사용한다고 알려준다.
        GameManager.instance.FireProjectile(caster.index, myNum, dir.Value);
    }

    public IEnumerator Fire(Character caster, Vector3 dir)
    {
        #region Wait for Pre delay
        if (targetType != TargetType.Auto) caster.usingSkill = true;
        // 스킬 발사 상태 변경 2020 01 30
        skillState = SkillState.PreDelay;
        yield return new WaitForSeconds(preDelay);
        #endregion

        #region 투사체 발사
        // 스킬 발사 상태 변경 2020 01 30
        skillState = SkillState.Fire;

        ProjectileInfo info = ProjectileInfo(caster, dir);
        Vector3 spawnPos = caster.transform.position + new Vector3(0, 1.1f, 0);

        Projectile projectile = UnityEngine.Object.Instantiate(proj, spawnPos, Quaternion.identity);
        projectile.Initialize(info);
        #endregion

        #region Wait for Post Delay
        yield return new WaitForSeconds(postDelay);
        if (targetType != TargetType.Auto) caster.usingSkill = false;
        #endregion

        skillState = SkillState.CoolDown;

        #region Cool Down...
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

    public void Initialize(int num)
    {
        skillState = SkillState.Idle;
        remainCool = 0f;
        myNum = num;
    }
}