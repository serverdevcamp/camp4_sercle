using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    // 옵션에서 관리용도의 마스터 볼륨
    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;
    [SerializeField]
    private AudioClip[] outGameClips;
    [SerializeField]
    private AudioClip[] inGameClips;
    // 음악 사전
    private Dictionary<string, AudioClip> audioClipDic;

    // 사운드 플레이어
    private AudioSource sfxPlayer;
    private AudioSource bgmPlayer;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        bgmPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        sfxPlayer = GetComponent<AudioSource>();

        RegisterAudioClips();
        InitPlayerSetting();
    }

    // Start is called before the first frame update
    void Start()
    {
     
    }

    private void RegisterAudioClips()
    {
        audioClipDic = new Dictionary<string, AudioClip>();
        outGameClips = Resources.LoadAll<AudioClip>("Audio/OutGame");
        inGameClips = Resources.LoadAll<AudioClip>("Audio/InGame");
        foreach (var item in outGameClips)
        {
            audioClipDic.Add(item.name, item);
        }
        foreach(var item in inGameClips)
        {
            audioClipDic.Add(item.name, item);
        }
    }

    private void InitPlayerSetting()
    {
        sfxPlayer.volume = masterVolumeSFX;
        bgmPlayer.volume = masterVolumeBGM;
    }

    // sfx 1회 재생.
    public void PlaySound(string clipName, float volume = 1f)
    {
        if(audioClipDic.ContainsKey(clipName) == false)
        {
            Debug.Log(clipName + " 이라는 클립은 없습니다. 재생 불가능.");
            return;
        }
        sfxPlayer.PlayOneShot(audioClipDic[clipName], volume * masterVolumeSFX);
    }

    // bgm 재생
    public void PlayBGM(string clipName, float volume = 1f)
    {
        if (audioClipDic.ContainsKey(clipName) == false)
        {
            Debug.Log(clipName + " 이라는 클립은 없습니다. 재생 불가능.");
            return;
        }
        bgmPlayer.clip = audioClipDic[clipName];
        bgmPlayer.loop = true;
        bgmPlayer.volume = volume;
        bgmPlayer.Play();
    }

    // bgm 멈춤
    public void StopBGM()
    {
        if(bgmPlayer.clip != null)
            bgmPlayer.Stop();
    }

    // bgm 다시재생
    public void RePlayBGM()
    {
        if (bgmPlayer.clip != null)
            bgmPlayer.Play();
    }
}
