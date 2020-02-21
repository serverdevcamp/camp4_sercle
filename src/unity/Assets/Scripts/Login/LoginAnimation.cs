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
                SoundManager.instance.PlaySound("ButtonClick");
            }
        }
        else if (pw.isFocused == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                id.Select();
                SoundManager.instance.PlaySound("ButtonClick");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                id.Select();
                SoundManager.instance.PlaySound("ButtonClick");
            }
        }
    }

    // 대포 애니메이션 재생
    private void PlayCanonAnim()
    {
        if (canon.localEulerAngles.x == 347.6f)
        {
            if (!canonBall.isPlaying)
            {
                StartCoroutine(PlayCanonSfx());
                canonBall.Play();
            }
        }
        else
            canonBall.Stop();
    }

    // 레이저 사운드 플레이.
    private IEnumerator PlayCanonSfx()
    {
        for (int i = 0; i < Random.Range(5, 7); i++)
        {
            SoundManager.instance.PlaySound("Login_Laser", Random.Range(0.2f, 0.3f));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
