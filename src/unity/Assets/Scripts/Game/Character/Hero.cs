using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class Hero : MonoBehaviour
{
    public enum State { Appear, Skill, DisAppear }

    [Header("Basic Info")]
    [SerializeField] private int index;
    [SerializeField] private bool is1P;
    [SerializeField] private State state;

    [Header("Skill Info")]
    [SerializeField] private Skill skill;
    public Skill GetSkill { get { return skill; } }
}
