using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum eTYPE_BGM
    {
        Main = 0,
        InGame,
    }
    [SerializeField] AudioClip[] bgmClips;

    public enum eTYPE_EFFECT
    {
        BUTTON = 0,
        LIGHT_BOOM,
    }
    [SerializeField] AudioClip[] EffectClips;

    AudioSource bgmPlayer;
    List<AudioSource> _ltEffPlayers;

    static SoundManager instance;

    public static SoundManager Instance    //프로시저 기법
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        //자기자신을 부름
        instance = this;

        bgmPlayer = GetComponent<AudioSource>();
        _ltEffPlayers = new List<AudioSource>();
    }

    void LateUpdate()
    {
        //생서되는 이펙트 브금을 제거하기 위한 함수
        foreach (AudioSource item in _ltEffPlayers)
        {
            if (item.isPlaying == false)
            {
                _ltEffPlayers.Remove(item);
                Destroy(item.gameObject);
                break;
            }
        }
    }
    public void PlayBGM(eTYPE_BGM type, float volum = 1.0f, bool isloop = true)
    {
        // 현재 재생 중인 BGM이 있을 경우 중복 재생하지 않도록 처리
        if (bgmPlayer.isPlaying && bgmPlayer.clip == bgmClips[(int)type])
        {
            return;
        }

        bgmPlayer.clip = bgmClips[(int)type];
        bgmPlayer.volume = volum;
        bgmPlayer.loop = isloop;

        bgmPlayer.Play();
    }

    public void PlayEffect_OnMng(eTYPE_EFFECT type, float volume = 1.0f, bool loop = false)
    {
        //Game Object 내의 EffectClips 가져옴.
        GameObject go = new GameObject("EffectClips");      //나 자신, 사운드 매니저에게 넣어줌
        //부모에게 달리게 만듦
        go.transform.SetParent(transform);
        //지금 만든 effect sound에 오디오 소스를 추가
        AudioSource AS = go.AddComponent<AudioSource>();
        //소스의 출력과 관련된 함수들.
        AS.clip = EffectClips[(int)type];
        AS.volume = volume;
        AS.loop = loop;

        AS.Play();
        //재생이 끝난 오디오는 파괴를 위해, 리스트로 보냄
        _ltEffPlayers.Add(AS);

    }

    public void PlayEffect(GameObject obj, eTYPE_EFFECT type, float volume = 1.0f, bool loop = false)
    {
        GameObject go = new GameObject("EffectClips");
        go.transform.SetParent(obj.transform);
        //로컬 포지션을 0,0,0으로 함. => 해당 오브젝트(obj)에 붙음.
        go.transform.localPosition = Vector3.zero;


        AudioSource AS = go.AddComponent<AudioSource>();
        //소스의 출력과 관련된 함수들.
        AS.clip = EffectClips[(int)type];
        AS.volume = volume;
        AS.loop = loop;

        AS.Play();
        //재생이 끝난 오디오는 파괴를 위해, 리스트로 보냄
        _ltEffPlayers.Add(AS);
    }
}
