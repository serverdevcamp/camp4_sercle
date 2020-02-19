using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileInfo
{
    public int casterCampNum;
    public Vector3 direction;
    public float speed;
    public float range;
    public float size;
    public TargetType targetType;
    public TargetNum targetNum;
    public List<SkillEffect> skillEffects;

    public ProjectileInfo(int casterCampNum, Vector3 direction, float speed, float range,
                        float size, TargetType targetType, TargetNum targetNum,
                        List<SkillEffect> skillEffects)
    {
        this.casterCampNum = casterCampNum;
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
    [SerializeField] private int casterCampNum;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private TargetType targetType;
    [SerializeField] private TargetNum targetNum;
    [SerializeField] private List<SkillEffect> skillEffects;

    public void Initialize(ProjectileInfo info)
    {
        casterCampNum = info.casterCampNum;
        direction = info.direction;
        speed = info.speed;
        range = info.range;
        transform.localScale = Vector3.one * info.size;
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
        Robot target = other.GetComponent<Robot>();
        if (!target) return;

        if (IsValidTargetType(target) == false) return;

        foreach(SkillEffect effect in skillEffects)
        {
            GameManager.instance.RequestSkillEffect(target, effect);
            // 투사체 히트 이펙트 적용
            // target.GetComponent<EffectController>().OnHitVFX(effect, caster.index);
        }
        if (targetNum == TargetNum.One) Destroy(gameObject);
    }

    private bool IsValidTargetType(Robot target)
    {
        if (targetType == TargetType.Friend && casterCampNum == target.CampNum) return true;
        if (targetType == TargetType.Enemy && casterCampNum != target.CampNum) return true;

        return false;
    }
}