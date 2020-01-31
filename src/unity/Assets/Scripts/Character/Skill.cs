using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RangeType { Self, Around, Direction }
public enum TargetType { Auto, Self, Friend, Enemy }
public enum TargetNum { One, All }

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Skill")]
public class Skill : ScriptableObject
{
    public enum SkillState { Idle, Ready, PreDelay, Fire, PostDelay, CoolDown }

    [Header("Basic Info")]
    public string skillName;
    public string description;
    public SkillState skillState;
    public int myNum;

    [Header("Time")]
    public float preDelay;
    public float postDelay;
    public float coolDown;
    private float remainingCool;
    public float RemaingCool { get { return remainingCool; } }

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
            while (dir.HasValue == false)
            {
                bool isValid = true;
                dir = GameManager.instance.GetDirection(caster, ref isValid);

                if (isValid == false) yield break;

                yield return new WaitForFixedUpdate();
            }
        }
        #endregion

        // skillState = SkillState.Fire;

        Debug.Log("Use " + skillName + " to " + dir);

        #region Wait for Pre delay
        if (targetType != TargetType.Auto) caster.usingSkill = true;
        // 스킬 발사 상태 변경 2020 01 30
        skillState = SkillState.PreDelay;
        yield return new WaitForSeconds(preDelay);
        #endregion

        #region 투사체 발사
        // GM에게 index 번째 캐릭터의 num번째 스킬을 dir 방향으로 사용한다고 알려준다.
        GameManager.instance.FireProjectile(caster.index, myNum, dir.Value);
        // 스킬 발사 상태 변경 2020 01 30
        skillState = SkillState.Fire;
        #endregion

        #region Wait for Post Delay
        yield return new WaitForSeconds(postDelay);
        if (targetType != TargetType.Auto) caster.usingSkill = false;
        #endregion

        skillState = SkillState.CoolDown;

        #region Cool Down...
        remainingCool = 0;

        while (remainingCool < coolDown)
        {
            remainingCool += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        remainingCool = coolDown;
        #endregion

        skillState = SkillState.Idle;
    }

    public void Fire(Character caster, Vector3 dir)
    {
        Vector3 spawnPos = caster.transform.position + new Vector3(0, 1.1f, 0);
        Projectile projectile = Instantiate(proj, spawnPos, Quaternion.identity);
        projectile.Initialize(caster, dir, speed, range, size, targetType, targetNum, skillEffects);
    }
}
