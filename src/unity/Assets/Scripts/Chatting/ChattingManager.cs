/*
 * 로비에서의 채팅창을 관리하는 매니저
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChattingManager : MonoBehaviour
{
    // 입력창
    public InputField inputField;

    // 역대 채팅 내역을 기록할 문자열
    public Text dialogue;

    // 스크롤 바
    public ScrollRect scrollbar;

    // Start is called before the first frame update
    void Start()
    {
        dialogue.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // 아무 입력이 없다면
        if(inputField.text.Length == 0)
        {
            return;
        }

        // 입력 텍스트의 마지막 문자가 엔터라면
        if (Input.GetKeyDown(KeyCode.Return))
        {
            dialogue.text += inputField.text;
            dialogue.text += "\n";
            inputField.text = "";
            scrollbar.verticalNormalizedPosition = 0f;
        }
        
    }

    

}
