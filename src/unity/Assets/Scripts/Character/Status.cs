using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { None, MHP, CHP, SPD, ATK, DEF, CRT, DDG, CC }
public enum HardCCType { None, Stun }

[System.Serializable]
public class Status
{
    [SerializeField] private int maxHp;
    [SerializeField] private int curHp;
    [SerializeField] private float spd;
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private float crt;
    [SerializeField] private float ddg;
    [Tooltip("Critical Coefficient. 치명타 계수")]
    [SerializeField] private float cc;
    [SerializeField] private HardCCType hardCC;

    public int MHP { get { return maxHp; } }
    public int CHP { get { return curHp; } }
    public float SPD { get { return spd; } }
    public float ATK { get { return atk; } }
    public float DEF { get { return def; } }
    public float CRT { get { return crt; } }
    public float DDG { get { return ddg; } }
    public float CC { get { return cc; } }
    public HardCCType HardCC { get { return hardCC; } }

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
            case StatusType.CRT:
                return CRT;
            case StatusType.DDG:
                return DDG;
            case StatusType.CC:
                return CC;
        }
        Debug.LogError("Something wrong during getting status value.");
        return 0;
    }

    /// <summary>
    /// 해당 유닛의 Status를 amount만큼 변경합니다. amount가 양수일 경우 더해지고 음수일 경우 뺍니다.
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
                break;
            case StatusType.SPD:
                spd += amount;
                break;
            case StatusType.ATK:
                atk += amount;
                break;
            case StatusType.DEF:
                def += amount;
                break;
            case StatusType.CRT:
                crt += amount;
                break;
            case StatusType.DDG:
                ddg += amount;
                break;
            case StatusType.CC:
                cc += amount;
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
            case StatusType.CRT:
                crt = target;
                break;
            case StatusType.DDG:
                ddg = target;
                break;
            case StatusType.CC:
                cc = target;
                break;
            default:
                Debug.LogError("Something is Wrong at Changing Status");
                break;
        }
    }

    public void ApplyCC(HardCCType ccType)
    {
        hardCC = ccType;
    }
}