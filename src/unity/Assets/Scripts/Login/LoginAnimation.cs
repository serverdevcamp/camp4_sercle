using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginAnimation : MonoBehaviour
{
    public Transform canon;
    public ParticleSystem canonBall;

    public InputField id;
    public InputField pw;
    public Button logIn;

    private void Update()
    {
        // pw, id 입력 받은 후 엔터키 입력시 버튼 활성화
        if (Input.GetKeyDown(KeyCode.Return) && pw.text.Length > 0 && id.text.Length > 0)
        {
            logIn.Select();
        }
    }

    private void LateUpdate()
    {
        PlayCanonAnim();
        OnTabInput();
    }

    // 탭키로 UI 이동
    private void OnTabInput()
    {
        if (id.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                pw.Select();
            }
        }
        else if (pw.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                id.Select();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                id.Select();
            }
        }
    }

    // 대포 애니메이션 재생
    private void PlayCanonAnim()
    {
        if (canon.localEulerAngles.x == 347.6f)
        {
            if (!canonBall.isPlaying)
                canonBall.Play();
        }
        else
            canonBall.Stop();
    }
}
