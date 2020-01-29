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
    [SerializeField] private SkillEffect skillEffect;

    public void Initialize(
        Character caster, Vector3 direction, float speed, float range,
        Vector3 size, TargetType targetType, TargetNum targetNum, SkillEffect skillEffect)
    {
        this.caster = caster;
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        transform.localScale = size;
        this.targetType = targetType;
        this.targetNum = targetNum;
        this.skillEffect = skillEffect;
    }

    private void Start()
    {
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

        GameManager.instance.ApplySkill(target, skillEffect.GetEffectResult(caster, target));

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