using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileInfo
{
    public Character caster;
    public Vector3 direction;
    public float speed;
    public float range;
    public Vector3 size;
    public TargetType targetType;
    public TargetNum targetNum;
    public List<SkillEffect> skillEffects;

    public ProjectileInfo(Character caster, Vector3 direction, float speed, float range,
                        Vector3 size, TargetType targetType, TargetNum targetNum,
                        List<SkillEffect> skillEffects)
    {
        this.caster = caster;
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        this.size = size;
        this.targetType = targetType;
        this.targetNum = targetNum;
        this.skillEffects = skillEffects;
    }
}

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

    public void Initialize(ProjectileInfo info)
    {
        caster = info.caster;
        direction = info.direction;
        speed = info.speed;
        range = info.range;
        transform.localScale = info.size;
        targetType = info.targetType;
        targetNum = info.targetNum;
        skillEffects = info.skillEffects;
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

        if (IsValidTargetType(target) == false) return;

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
}