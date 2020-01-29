/*
 * 캐릭터 정보와 마우스 포인터의 정보는 매 프레임 동기화
 * 
 * 프레임데이터와 스킬/공격 데이터는?
 */ 

using UnityEngine;
using System.Collections;










public class InputManager : MonoBehaviour
{

    MouseData[] syncedMouseInputs = new MouseData[2]; //동기화된 마우스 입력값.
    MouseData localMouseInput; //현재 마우스 입력값(이 값을 송신시킨다).


    // 기존에서 추가
    InputData[] syncedInputs = new InputData[2];

    // 현재 캐릭터 데이터 값(3마리)
    // CharacterData[] localCharacterInfos = new CharacterData[3];
    // 로컬 캐릭터들의 참조
    // Player[] players = new Player[3];

    private void Start()
    {
        // Debug.Log("InputManager의 Start에서, 로컬 캐릭터 3개의 참조를 지정해야 함.");
        // players[0] = GameObject.Find("Player_0").GetComponent<Player>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Debug.Log(gameObject.name + Time.frameCount.ToString() + " scale:" + Time.timeScale.ToString());

        localMouseInput.mouseButtonLeft = Input.GetMouseButton(0);
        localMouseInput.mouseButtonRight = Input.GetMouseButton(1);


        //마우스 좌표 계산.
        //그대로 넣으면 윈도우 크기 차이로 곤란해지니 변환합니다.
        Vector3 pos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(pos);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float depth;
        plane.Raycast(ray, out depth);

        Vector3 worldPos = ray.origin + ray.direction * depth;

        localMouseInput.mousePositionX = worldPos.x;
        localMouseInput.mousePositionY = worldPos.y;
        localMouseInput.mousePositionZ = worldPos.z;

       



    }

    // 이 클라이언트가 소유한 유닛(원격 유닛이 아닌 로컬 유닛 index)의 정보를 반환 
    public CharacterData GetLocalCharData(int index)
    {
        CharacterData data = new CharacterData();
        return data;
    }



    //현재 입력값을 반환합니다.
    public MouseData GetLocalMouseData()
    {
        return localMouseInput;
    }

    //동기화된 입력값을 반환합니다.
    public MouseData GetMouseInputData(int id)
    {
        //		Debug.Log("id:" + id + "' " + inputData.Length);
        return syncedMouseInputs[id];
    }

    //동기화된 입력값 설정용.
    public void SetMouseInputData(int id, MouseData data)
    {
        syncedMouseInputs[id] = data;
    }

}