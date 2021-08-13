using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DG.Tweening;

//==================// //==================//
public enum SoundKey
{
    // BGM
    BGM1, BGM2, BGM3,
    // EFF1
    WALK, JUMP, LAND,
    SKILL1, SKILL2, NOENERGY,
    DEAD, AIRJUMP, SHIELD,
    // EFF2
    FLIP, OBTAIN, BOARDDESTROY,
    ITEM_SHOW, ITEM_OBTAIN,
    // EFF3
    WARNING, EXPLOSION,
    GHOST_TALK, GHOST_DEAD,
    THUNDER_GOING, THUNDER_READY, THUNDER_EXPLOSION,
    CONCRETE_GOING, CONCRETE_CRASH,
    SHOOTER_NARROW, SHOOTER_FASTER,
    // EFF4
    MOUSE_BTN_UP, MOUSE_BTN_DOWN, MOUSE_ON,
    DOOR_MOVE, LOGO
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
    public AudioMixer mixer;
    public ClipInfo[] clipInfos;
    //
    // [Range(0.0f, 1.0f)]
    // [Header("BGM Option")]
    //
    [Header("FX Option")]
    public int audioPoolCount = 50;
    public int soundDistanceMin = 5;
    public int soundDistanceMax = 10;
    //
    private Dictionary<SoundKey, AudioClip> clips = new Dictionary<SoundKey, AudioClip>();
    //
    private AudioSource bgAuido;
    private List<AudioSource> fxAudios = new List<AudioSource>();
    //
    public Slider bgrSlider;
    public Slider effSlider;
    //==============================================//
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // CreateAudio();
            // DontDestroyOnLoad(this.gameObject); // only for root
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //
    private void Start()
    {
        CreateAudio();
    }
    //
    private void Update()
    {
        UpdatePitchShifter();
    }
    //==============================================//
    public void SetBGRPitch(int lv)
    {
        if (bgAuido == null)
            return;
        //
        float value = 1.0f + (float)lv * 0.2f;
        bgAuido.pitch = value;
    }
    //
    private void UpdatePitchShifter()
    {
        if (bgAuido == null)
            return;
        //
        if (bgAuido.isPlaying == true)
        {
            float val = 1.0f / bgAuido.pitch;
            mixer.SetFloat("pitchShifter_pitchValue", val);
        }
    }
    //
    // ====================== Event : Slider ====================== //
    public void SetSliders()
    {
        if (bgrSlider == null)
            bgrSlider = GameObject.Find("Slider_BGR").GetComponent<Slider>();
        if (effSlider == null)
            effSlider = GameObject.Find("Slider_EFF").GetComponent<Slider>();
    }
    //
    public void Slider_OnPointerDrag_BGR()
    {
        if (bgrSlider == null)
            SetSliders();
        //
        // PlayBGR(SoundKey.BGM1);
        int bgNum = SceneControl.instance.currentBGR;
        switch (bgNum)
        {
            case 0:
                PlayBGR(SoundKey.BGM1);
                break;
            case 1:
                PlayBGR(SoundKey.BGM2);
                break;
            case 2:
                PlayBGR(SoundKey.BGM3);
                break;
            default:
                break;
        }
    }
    //
    public void Slider_OnPointerUp_BGR()
    {
        if (bgrSlider == null)
            SetSliders();
        //
        // bgrVolume = bgrSlider.value;
        SceneControl.instance.SaveOption_Sound();
        //
        string curSceneName = SceneManager.GetActiveScene().name;
        if (curSceneName != "MainScene")
            StopBGR();
    }
    //
    public void Slider_OnPointerUp_EFF()
    {
        if (effSlider == null)
            SetSliders();
        //
        //effVolume = effSlider.value;
        SceneControl.instance.SaveOption_Sound();
        //
        Play(SoundKey.OBTAIN, null, effSlider.value);
    }
    //==============================================//
    public void SetSoundAndSliderFromSaveData()
    {
        bgrSlider.value = (float)SceneControl.instance.saveData.bgrVolume;
        effSlider.value = (float)SceneControl.instance.saveData.effVolume;
        // Set AudioMixer
        mixer.SetFloat("bgrVolume", bgrSlider.value);
        mixer.SetFloat("effVolume", effSlider.value);
    }
    //
    public void SetVolumeBySlider()
    { // Called By Slider
        string curSceneName = SceneManager.GetActiveScene().name;
        if (curSceneName == "0_LogoScene")
            return;
        //
        if (bgrSlider == null || effSlider == null)
            SetSliders();
        // -40(0) ~ 0(1)
        if (bgrSlider.value == -40.0f)
            mixer.SetFloat("bgrVolume", -80.0f);
        else
            mixer.SetFloat("bgrVolume", bgrSlider.value);
        //
        if (effSlider.value == -40.0f)
            mixer.SetFloat("effVolume", -80.0f);
        else
            mixer.SetFloat("effVolume", effSlider.value);
    }
    //==============================================//
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
        bgAuido.priority = 0;
        bgAuido.outputAudioMixerGroup = mixer.FindMatchingGroups("BGR")[0];
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
            audio.spatialBlend = 0.8f;
            audio.outputAudioMixerGroup = mixer.FindMatchingGroups("EFF")[0];

            fxAudios.Add(audio);

            obj.SetActive(false);
        }
    }
    //
    public void SetAudioSource(AudioSource audio)
    {
        audio.playOnAwake = false;
        audio.minDistance = soundDistanceMin;
        audio.maxDistance = soundDistanceMax;
        audio.spatialBlend = 0.8f;
        audio.outputAudioMixerGroup = mixer.FindMatchingGroups("EFF")[0];
    }
    //
    public void ResetBGRPitch()
    {
        if (bgAuido == null)
            return;
        //
        bgAuido.pitch = 1.0f;
        mixer.SetFloat("pitchShifter_pitchValue", 1.0f);
    }
    //
    public void RePlayBGR()
    {
        StopBGR();
        //
        int bgNum = SceneControl.instance.currentBGR;
        switch (bgNum)
        {
            case 0:
                PlayBGR(SoundKey.BGM1);
                break;
            case 1:
                PlayBGR(SoundKey.BGM2);
                break;
            case 2:
                PlayBGR(SoundKey.BGM3);
                break;
            default:
                break;
        }
    }
    //
    public void StopBGR()
    {
        if (bgAuido == null)
            return;
        //
        bgAuido.Stop();
    }
    //
    public void PlayBGR(SoundKey key)
    {
        if (clips.ContainsKey(key) == false)
            return;
        //
        bgAuido.clip = clips[key];
        //
        if (bgAuido.isPlaying == false)
            bgAuido.Play();
    }
    //
    public void PlayBTNClick()
    {
        Play(SoundKey.MOUSE_BTN_DOWN);
        Play(SoundKey.MOUSE_BTN_UP);
    }
    //
    public void Play(SoundKey key, int audioIndex, bool loop = false)
    {
        AudioSource playAudio = null;
        playAudio = fxAudios[(fxAudios.Count - 1) - audioIndex];
        //
        if(loop == true)
            if (playAudio.isPlaying == true)
                return;
        //
        playAudio.gameObject.SetActive(true);
        playAudio.PlayOneShot(clips[key]);
        AudioDisable(playAudio, clips[key].length);
    }
    //
    public void Play(SoundKey key, Transform target = null, float effVol = -1.0f)
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
        if (target != null)
            playAudio.transform.SetParent(target);
        //
        playAudio.transform.localPosition = Vector3.zero;
        //
        playAudio.PlayOneShot(clips[key]);
        //
        AudioDisable(playAudio, clips[key].length);
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
        playAudio.PlayOneShot(clips[key]);
        //
        AudioDisable(playAudio, clips[key].length);
    }
    //
    private void AudioDisable(AudioSource audio, float time)
    {
        Sequence open = DOTween.Sequence()
                .AppendInterval(time)
                .AppendCallback(() => { audio.transform.SetParent(transform); })
                .AppendCallback(() => { audio.gameObject.SetActive(false); })
                .SetUpdate(true)
                ;
    }
    //
    public void ResetFXAudios()
    {
        foreach (AudioSource audio in fxAudios)
        {
            if (audio.gameObject.activeSelf == true)
            {
                if (audio.isPlaying == false)
                {
                    audio.transform.SetParent(transform);
                    audio.gameObject.SetActive(false);
                }
            }
        }
    }
}