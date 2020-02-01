using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private Character caster;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private TargetType targetType;
    [SerializeField] private TargetNum targetNum;
    [SerializeField] private List<SkillEffect> skillEffects;

    public void Initialize(
        Character caster, Vector3 direction, float speed, float range,
        Vector3 size, TargetType targetType, TargetNum targetNum, List<SkillEffect> skillEffects)
    {
        this.caster = caster;
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        transform.localScale = size;
        this.targetType = targetType;
        this.targetNum = targetNum;
        this.skillEffects = skillEffects;
    }

    private void FixedUpdate()
    {
        if (range > 0)
        {
            transform.position += direction * speed * Time.fixedDeltaTime;
            range -= speed * Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Character target = other.GetComponent<Character>();
        if (!target) return;

        // if (IsValidTargetType(target) == false) return;
        // 2020 02 01 기존의 IsValidTargetType 함수 대신 리모트 캐릭터가 스킬 쓰는 경우까지 고려한 IsValidTarget 함수를 사용.
        if (IsValidTarget(target) == false) return;

        foreach(SkillEffect effect in skillEffects)
        {
            GameManager.instance.ApplySkill(target, effect.GetEffectResult(caster, target));
        }

        if (targetNum == TargetNum.One) Destroy(gameObject);
    }

    private bool IsValidTargetType(Character target)
    {
        switch (targetType)
        {
            case TargetType.Auto:
                if (target == caster) return false;
                if (!target.isFriend) return true;
                break;
            case TargetType.Self:
                if (target == caster) return true;
                break;
            case TargetType.Friend:
                if (target == caster) return false;
                if (target.isFriend) return true;
                break;
            case TargetType.Enemy:
                if (target == caster) return false;
                if (!target.isFriend) return true;
                break;
        }

        return false;
    }

    // 2020 02 01 리모트 캐스터를 고려한 유효타겟 판별 함수.
    // 이 함수는 projectile이 캐릭터와 부딪혔을때 call 되므로, 각자의 단말에서 처리된다.
    private bool IsValidTarget(Character target)
    {
        // caster가 isFriend = true. 즉 이 함수가 처리되는 단말에서 로컬 캐릭터라면
        if (caster.isFriend)
        {
            switch (targetType)
            {
                case TargetType.Auto:
                    if (target == caster) return false; // 시전자는 자기가 사용한 기본공격을 맞을 수 없음.
                    if (!target.isFriend) return true;  // 로컬이 시전하고 리모트가 맞은 경우.
                    break;
                case TargetType.Self:
                    if (target == caster) return true;  // 로컬이 시전하고 맞은것도 로컬 자신
                    break;
                case TargetType.Friend:
                    if (target == caster) return false; // 아군에게 쓴 스킬은 자기가 맞으면 안됨.
                    if (target.isFriend) return true;   // 로컬이 쓴 스킬이 로컬캐릭터들에게 맞은경우
                    break;
                case TargetType.Enemy:
                    if (target == caster) return false; // 본인이 스킬쓰고 본인이 맞을 수 없음
                    if (!target.isFriend) return true;  // 적(리모트)가 맞은경우
                    break;
            }
            return false;
        }
        // 이 함수가 처리되는 단말에서 caster가 리모트 캐릭터라면
        else
        {
            switch (targetType)
            {
                case TargetType.Auto:
                    if (target == caster) return false;
                    if (target.isFriend) return true;   // 시전자가 리모트, 맞은애가 로컬이면 유효한 공격.
                    break;
                case TargetType.Self:
                    if (target == caster) return true;
                    break;
                case TargetType.Friend:
                    if (target == caster) return false;
                    if (!target.isFriend) return true;  // 시전자가 리모트고, 맞은애들도 리포트인 경우 유효.
                    break;
                case TargetType.Enemy:
                    if (target == caster) return false;
                    if (target.isFriend) return true;   // 시전자가 리모트고, 맞은애는 로컬인경우.
                    break;
            }
            return false;
        }
    }

}