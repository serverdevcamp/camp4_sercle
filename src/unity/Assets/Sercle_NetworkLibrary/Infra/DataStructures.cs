using UnityEngine;
using System.Collections;

// 패킷 데이터 식별용 열거형
// 모든 경우의 request, response, Data를 붙여야함.
public enum PacketId
{
    CharacterData = 0,
    SkillData,
    MovingData,
    MatchingData,
    MatchingResponse,
    MatchingDecision,
    MatchingRetry,
    MatchingComplete,
    MatchingReject,
    MatchingCancel,
    GameServerJoin,
    SyncData,
    GameServerEnd,
    SelectedSkillData,
};

//매칭 패킷 데이터
public enum MatchingPacketId
{
    MatchingRequest = 0,
    MatchingResponse,
    MatchingCatch,
};

public enum GamePacketId
{
    NormalEnd = 0,
    OpponentEnd,

}
//매칭 요청 결과
public enum MatchingResult
{
    Success = 0,
    Fail,
}
// 스킬 종류 식별용 열거형

public enum MatchingDecision
{
    Accept = 0,
    Reject,
}

//매칭 요청 정보
public struct MatchingData
{
    //유저 고유 번호
    public MatchingPacketId request;
    public override string ToString()
    {
        string str = "";
        str += "matchingpacketid:" + request;
        return str;
    }
}

//서버->클라이언public struct GameEndData
{
    public GamePacketId request;
}
public struct GameJoinData
{
    public int id;
    public int roomNum;
}
public struct MatchingCancelData
{
    public int myInfo;
}
//상대방이 매칭 거절 했을시 전달할 데이터
//서버 -> 클라이언트
public struct MatchingRetryData
{
    public MatchingResult result;
}
//매칭 응답 정보
public struct MatchingResponseData
{
    public MatchingPacketId request;
    public MatchingResult result;
    public int myInfo;
    public override string ToString()
    {
        string str = "";
        str += "matchingpacketid:" + request;
        str += " matchingresult:" + result;
        return str;
    }
}

public struct MatchingDecisionData
{
    public MatchingDecision decision;
    public int myinfo;
}
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

// 동기화 데이터
public struct SyncData
{
    public long sendTime;
    public override string ToString()
    {
        string str = "";
        str += "Sendtime:" + sendTime;
        return str;
    }
}

public struct MatchingCompleteData
{
    public int roomId;
    public int playerCamp;
    public int myInfo;
}

public struct MatchingRejectData
{
    public MatchingResult result;
}
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
    // 로봇이 사용한 공격인가?
    public bool isRobot;
    // 캐릭터 번호
    public int index;
    // 방향
    public float dirX;
    public float dirY;
    public float dirZ;

    public override string ToString()
    {
        string str = "";
        str += "isRobot:" + isRobot;
        str += " index:" + index;
        str += " dirX:" + dirX;
        str += " dirY:" + dirY;
        str += " dirZ:" + dirZ;
        return str;
    }
}

// 스킬 선택 씬에서 선택한 스킬 데이터
public struct SelectedSkillData
{
    public int userId;  // MatchingManager.instance.myInfo;
    public int[] skillIndex;
}

// 스킬 정보 Json 배열
public class SkillInfoJsonArray
{
    public SkillInfoJson[] skillInfo;
}

// 스킬정보 Json
[System.Serializable]
public class SkillInfoJson
{
    public int skillNumber;
    public string skillName;
    public string skillDesc;
    public string skillImagePath;
    public string skillEffectPath;
}