using UnityEngine;
using System.Collections;

// 패킷 데이터 식별용 열거형
public enum PacketId
{
    CharacterData = 0,
    SkillData,
    MovingData
};

// 스킬 종류 식별용 열거형


// 데이터의 헤더에 패킷을 붙힌다.
// Fix this : 네트워크에 사용할 구조체는 한곳에 몰아놓기.
public struct PacketHeader
{
    // 패킷 ID
    public int packetId;
};


// 마우스 정보 데이터
public struct MouseData
{
    public int frame;
    public bool mouseButtonLeft;
    public bool mouseButtonRight;

    public float mousePositionX;
    public float mousePositionY;
    public float mousePositionZ;

    public override string ToString()
    {
        string str = "";
        str += "frame:" + frame;
        str += " mouseButtonLeft:" + mouseButtonLeft;
        str += " mouseButtonRight:" + mouseButtonRight;
        str += " mousePositionX:" + mousePositionX;
        str += " mousePositionY:" + mousePositionY;
        str += " mousePositionZ:" + mousePositionZ;
        return str;
    }

};

// 일반공격, 스킬 포함 데이터
// 
public struct AttackData
{
    public int frame;
    // 대상의 인덱스
    public int targetIndex;
    // 시전자의 인덱스
    public int casterIndex;
    // 스킬 타입 (열거형으로 교체하기)
    public int skillType;
    // 총 데미지 (데미지량&힐량, 버프/디버프의 경우 value는 0)
    // target의 단말에서 skillType에 맞춰 처리하면 됨.
    public float totalValue;
};

// 프레임 맞추기용 데이터
public struct FrameData
{
    public int frame;
};

// 캐릭터 정보 동기화 데이터
public struct CharacterData
{
    public int frame;
    // 어떤 플레이어를 상대 단말과 동기화 시킬지 지정
    public int playerIndex;
    // 최대 체력
    public float mhp;
    // 현재 체력
    public float chp;
    // 스피드
    public float spd;
    // 공격력
    public float atk;
    // 방어력
    public float def;
    // 크리티컬 
    public float crt;
    // 회피
    public float ddg;
    // 치명계수
    public float cc;

    public override string ToString()
    {
        string str = "";
        str += "frame:" + frame;
        str += " player index:" + playerIndex;
        str += " mhp:" + mhp;
        str += " chp:" + chp;
        str += " spd:" + spd;
        str += " atk:" + atk;
        str += " def:" + def;
        str += " crt:" + crt;
        str += " ddg:" + ddg;
        str += " cc:" + cc;
        return str;
    }
};

public struct InputData
{
    public int count;       // 데이터 수. 
    public int flag;        // 접속 종료 플래그.
    public MouseData[] datum;		// 키입력 정보.

    /*
    // 기존에서 추가로, Inputdata가 어떤 데이터를 가지고 있는지 기록
    public int dataType;
    public int frame;
    public FrameData[] frameDatum;
    public CharacterData[] charDatum;
    */
};



// 이동 정보
public struct MovingData
{
    // 캐릭터 번호
    public int index;
    // 좌표
    public float destX;
    public float destY;
    public float destZ;

    public override string ToString()
    {
        string str = "";
        str += "index:" + index;
        str += " destX:" + destX;
        str += " destY:" + destY;
        str += " destZ:" + destZ;
        return str;
    }
};


// 스킬 정보
public struct SkillData
{
    // 캐릭터 번호
    public int index;
    // 스킬 번호
    public int num;
    // 방향
    public float dirX;
    public float dirY;
    public float dirZ;

    public override string ToString()
    {
        string str = "";
        str += "index:" + index;
        str += " num:" + num;
        str += " dirX:" + dirX;
        str += " dirY:" + dirY;
        str += " dirZ:" + dirZ;
        return str;
    }
}