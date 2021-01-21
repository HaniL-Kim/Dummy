using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================// //==================//
public enum SoundKey
{
    BGM,
    JUMP, LAND,
}
//==================// //==================//
[System.Serializable]
public struct ClipInfo
{
    public SoundKey key;
    public AudioClip clip;
}
//==================// //==================//
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    //==============================================//
    public ClipInfo[] clipInfos;
    //
    [Header("BGM Option")]
    [Range(0.0f, 1.0f)]
    public float bgmVolume = 1.0f;
    //
    [Header("FX Option")]
    [Range(0.0f, 1.0f)]
    public float fxVolume = 1.0f;
    public int audioPoolCount = 30;
    public int soundDistanceMin = 5;
    public int soundDistanceMax = 10;
    //
    private Dictionary<SoundKey, AudioClip> clips = new Dictionary<SoundKey, AudioClip>();
    //
    private AudioSource bgAuido;
    private List<AudioSource> fxAudios = new List<AudioSource>();
    //==============================================//
    private void Awake()
    {
        instance = this;
        //
        CreateAudio();
    }
    //
    private void CreateAudio()
    {
        foreach (ClipInfo clipInfo in clipInfos)
        {
            clips.Add(clipInfo.key, clipInfo.clip);
        }
        //
        CreateBGAudio();
        CreateFXAudio();
    }
    //
    private void CreateBGAudio()
    {
        GameObject obj = new GameObject("BGAudio");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        //
        bgAuido = obj.AddComponent<AudioSource>();
        bgAuido.loop = true;
        bgAuido.playOnAwake = false;
        bgAuido.volume = bgmVolume;
    }
    //
    private void CreateFXAudio()
    {
        for (int i = 0; i < audioPoolCount; i++)
        {
            GameObject obj = new GameObject("FXAudio" + i);
            obj.transform.SetParent(transform);
            AudioSource audio = obj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.minDistance = soundDistanceMin;
            audio.maxDistance = soundDistanceMax;
            audio.volume = fxVolume;
            audio.spatialBlend = 0.8f;

            fxAudios.Add(audio);

            obj.SetActive(false);
        }
    }
    //
    public void PlayBGM(SoundKey key)
    {
        if (clips.ContainsKey(key) == false)
            return;
        //
        bgAuido.clip = clips[key];
        bgAuido.volume = bgmVolume;
        bgAuido.Play();
    }
    //
    public void Play(SoundKey key, Transform target = null)
    {
        if (clips.ContainsKey(key) == false)
            return;
        //
        AudioSource playAudio = null;
        //
        foreach (AudioSource audio in fxAudios)
        {
            if (audio.gameObject.activeSelf == false)
            {
                playAudio = audio;
                break;
            }

        }
        if (playAudio == null)
            return;
        //
        playAudio.gameObject.SetActive(true);
        //
        if (transform == null)
            playAudio.transform.SetParent(Camera.main.transform);
        else
            playAudio.transform.SetParent(target);
        //
        playAudio.transform.localPosition = Vector3.zero;
        //
        playAudio.PlayOneShot(clips[key]);
        //
        StartCoroutine(AudioDisable(playAudio, clips[key].length));
    }
    //
    public void Play(SoundKey key, Vector3 pos)
    {
        if (clips.ContainsKey(key) == false)
            return;
        //
        AudioSource playAudio = null;
        //
        foreach (AudioSource audio in fxAudios)
        {
            if (audio.gameObject.activeSelf == false)
            {
                playAudio = audio;
                break;
            }
        }
        //
        if (playAudio == null)
            return;
        //
        playAudio.gameObject.SetActive(true);
        //
        playAudio.transform.position = pos;
        //
        playAudio.PlayOneShot(clips[key]);
        //
        StartCoroutine(AudioDisable(playAudio, clips[key].length));
    }
    //
    private IEnumerator AudioDisable(AudioSource audio, float time)
    {
        yield return new WaitForSeconds(time);
        //
        audio.transform.SetParent(transform);
        audio.gameObject.SetActive(false);
    }
}