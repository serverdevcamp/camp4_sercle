using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { None, MHP, CHP, SPD, ATK, DEF, CRT, DDG, CC }
public enum CCType { None, Stun }

[System.Serializable]
public class Status
{
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private float spd;
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private CCType cc;

    public int MHP { get { return maxHp; } }
    public int CHP { get { return curHp; } }
    public float SPD { get { return spd; } }
    public float ATK { get { return atk; } }
    public float DEF { get { return def; } }
    public CCType CC { get { return cc; } }

    public float Value(StatusType type)
    {
        switch (type)
        {
            case StatusType.SPD:
                return SPD;
            case StatusType.ATK:
                return ATK;
            case StatusType.DEF:
                return DEF;
        }
        Debug.LogError("Something wrong during getting status value.");
        return 0;
    }

    /// <summary>
    /// 해당 유닛의 Status를 amount만큼 변경합니다.
    /// </summary>
    /// <param name="type">변경하고자 하는 StatusType</param>
    /// <param name="amount">변경하고자 하는 양</param>
    public void ChangeStat(StatusType type, float amount)
    {
        switch(type)
        {
            case StatusType.MHP:
                maxHp += (int)amount;
                break;
            case StatusType.CHP:
                curHp += (int)amount;
                if (curHp > maxHp) curHp = maxHp;
                break;
            case StatusType.SPD:
                spd += amount;
                if (spd < 0) spd = 0;
                break;
            case StatusType.ATK:
                atk += amount;
                if (atk < 0) atk = 0;
                break;
            case StatusType.DEF:
                def += amount;
                break;
            default:
                Debug.LogError("Something is Wrong at Changing Status");
                break;
        }
    }

    /// <summary>
    /// 해당 유닛의 Status를 target으로 변경합니다.
    /// </summary>
    /// <param name="type">변경하고자 하는 StatusType</param>
    /// <param name="target">목표 도달 수치</param>
    public void ChangeStatTo(StatusType type, float target)
    {
        switch (type)
        {
            case StatusType.MHP:
                maxHp = (int)target;
                break;
            case StatusType.CHP:
                curHp = (int)target;
                if (curHp > maxHp) curHp = maxHp;
                break;
            case StatusType.SPD:
                spd = target;
                break;
            case StatusType.ATK:
                atk = target;
                break;
            case StatusType.DEF:
                def = target;
                break;
            default:
                Debug.LogError("Something is Wrong at Changing Status");
                break;
        }
    }

    public void ApplyCC(CCType ccType)
    {
        cc = ccType;
    }
}