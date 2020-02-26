using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public enum RangeType { Around, Direction }
public enum TargetType { Friend, Enemy }
public enum TargetNum { One, All }

[System.Serializable]
public class Skill
{
    [Header("Basic Info")]
    public int skillNum;
    public string skillName;
    public string description;
    public Sprite image;
    public GameObject skillEffectPrefab;

    [Header("Time")]
    public float emergeDelay;   // 등장한 후 등장 포즈 취하는 시간
    public float preDelay;
    public float projDelay;
    public float postDelay;
    public float coolDown;
    public float remainCool;

    [Header("Projectile Info")]
    public Projectile proj;
    public float speed;
    public float range;
    public float size;
    public int tickCount;
    public float tickDelay;
    public TargetType targetType;
    public TargetNum targetNum;
    public List<SkillEffect> skillEffects;
}