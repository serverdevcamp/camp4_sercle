using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EffectResult
{
    public StatusType statusType;
    public HardCCType hardCCType;
    public float amount;
    public float duration;

    public EffectResult(StatusType type, float amount, float duration = 0)
    {
        statusType = type;
        hardCCType = HardCCType.None;
        this.amount = amount;
        this.duration = duration;
    }

    public EffectResult(HardCCType type, float duration)
    {
        statusType = StatusType.None;
        hardCCType = type;
        amount = 0;
        this.duration = duration;
    }
}

[CreateAssetMenu(fileName = "New Skill Effect", menuName = "Skill/Skill Effect")]
[System.Serializable]
public class SkillEffect : ScriptableObject
{
    public enum SkillType { Attack, Heal, Temp }
    public enum CCType { Soft, Hard }

    [SerializeField] private SkillType skillType;

    [SerializeField] private float coefficient;

    [SerializeField] private CCType ccType;
    [SerializeField] private StatusType statusType;
    [SerializeField] private HardCCType hardCCType;
    [SerializeField] private float amount;
    [SerializeField] private float duration;

    public List<EffectResult> GetEffectResult(Character caster, Character target)
    {
        List<EffectResult> effects = new List<EffectResult>();

        switch (skillType)
        {
            case SkillType.Attack:
                effects.AddRange(GetAttackResult(caster, target));
                break;
            case SkillType.Heal:
                effects.AddRange(GetHealResult(caster));
                break;
            case SkillType.Temp:
                effects.AddRange(GetTempResult());
                break;
        }

        return effects;
    }

    private List<EffectResult> GetAttackResult(Character caster, Character target)
    {
        List<EffectResult> effects = new List<EffectResult>();

        float damage = coefficient * caster.status.ATK * (1 - target.status.DEF / 100);
        int rand = Random.Range(0, 100);

        float prob = caster.status.CRT - target.status.DDG;

        if (prob > 0 && rand <= prob) damage *= caster.status.CC;
        else if (prob < 0 && rand <= Mathf.Abs(prob)) damage = 0;

        effects.Add(new EffectResult(StatusType.CHP, -damage));

        return effects;
    }

    private List<EffectResult> GetHealResult(Character caster)
    {
        List<EffectResult> effects = new List<EffectResult>();

        float healAmount = coefficient * caster.status.ATK;

        effects.Add(new EffectResult(StatusType.CHP, healAmount));

        return effects;
    }

    private List<EffectResult> GetTempResult()
    {
        List<EffectResult> effects = new List<EffectResult>();

        switch (ccType)
        {
            case CCType.Soft:
                effects.Add(new EffectResult(statusType, amount, duration));
                break;
            case CCType.Hard:
                effects.Add(new EffectResult(hardCCType, duration));
                break;
        }
        return effects;
    }
}