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
    public float activeDelay;
    public TargetType targetType;
    public TargetNum targetNum;
    public List<SkillEffect> skillEffects;
    public int heroSkillNum;

    public ProjectileInfo(int casterCampNum, Vector3 direction, float speed, float range,
                        float size, float activeDelay, TargetType targetType, TargetNum targetNum,
                        List<SkillEffect> skillEffects, int heroSkillNum = -1)
    {
        this.casterCampNum = casterCampNum;
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        this.size = size;
        this.activeDelay = activeDelay;
        this.targetType = targetType;
        this.targetNum = targetNum;
        this.skillEffects = skillEffects;
        this.heroSkillNum = heroSkillNum;
    }
}

[RequireComponent(typeof(BoxCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private int casterCampNum;
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float activeDelay;
    [SerializeField] private TargetType targetType;
    [SerializeField] private TargetNum targetNum;
    [SerializeField] private List<SkillEffect> skillEffects;
    [SerializeField] private int skillNumber;

    private bool isActive = false;

    public void Initialize(ProjectileInfo info)
    {
        casterCampNum = info.casterCampNum;
        direction = info.direction;
        speed = info.speed;
        range = info.range;
        activeDelay = info.activeDelay;
        transform.localScale = Vector3.one * info.size;
        targetType = info.targetType;
        targetNum = info.targetNum;
        skillEffects = info.skillEffects;
        skillNumber = info.heroSkillNum;
    }
    private void Start()
    {
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(WaitForActivate());
    }

    private void FixedUpdate()
    {
        if (isActive == false) return;

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

        if(skillNumber >= 0)
        {
            Debug.Log(skillNumber + " 스킬에 맞았습니다.");
        }

        bool isEffectActivated = false;

        foreach(SkillEffect effect in skillEffects)
        {
            GameManager.instance.RequestSkillEffect(target, effect);
            if (!isEffectActivated)
            {
                // 투사체 히트 이펙트 적용
                target.ShowHitEffect(skillNumber, effect);
                isEffectActivated = true;
            }
        }
        if (targetNum == TargetNum.One) Destroy(gameObject);
    }

    private bool IsValidTargetType(Robot target)
    {
        if (targetType == TargetType.Friend && casterCampNum == target.CampNum) return true;
        if (targetType == TargetType.Enemy && casterCampNum != target.CampNum) return true;

        return false;
    }

    private IEnumerator WaitForActivate()
    {
        if (activeDelay == 0)
        {
            isActive = true;
            GetComponent<BoxCollider>().enabled = true;
            yield break;
        }

        isActive = false;

        yield return new WaitForSeconds(activeDelay);

        isActive = true;
        GetComponent<BoxCollider>().enabled = true;
    }
}