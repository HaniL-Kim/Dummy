using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

//==================// //==================//
public enum SoundKey
{
    BGR,
    TEST,
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
    public AudioMixer mixer;
    public ClipInfo[] clipInfos;
    //
    [Header("BGM Option")]
    [Range(0.0f, 1.0f)]
    public float bgrVolume = 1.0f;
    //
    [Header("FX Option")]
    [Range(0.0f, 1.0f)]
    public float effVolume = 1.0f;
    public int audioPoolCount = 30;
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
        if(instance == null)
        {
            instance = this;
            //CreateAudio();
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //
    private void Update()
    {
        SetBGRPitch();
    }
    //==============================================//
    private void SetBGRPitch()
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
        if(bgrSlider == null)
            bgrSlider = GameObject.Find("Slider_BGR").GetComponent<Slider>();
        if(effSlider == null)
            effSlider = GameObject.Find("Slider_EFF").GetComponent<Slider>();
    }
    //
    public void Slider_OnPointerDrag_BGR(BaseEventData eventData)
    {
        if (bgrSlider == null)
            SetSliders();
        //
        PlayBGR(SoundKey.BGR, bgrSlider.value);
    }
    //
    public void Slider_OnPointerUp_BGR(BaseEventData eventData)
    {
        if (bgrSlider == null)
            SetSliders();
        //
        bgrVolume = bgrSlider.value;
        SceneControl.instance.SaveOption_Sound();
        //
        StopBGR();
    }
    //
    public void Slider_OnPointerUp_EFF(BaseEventData eventData)
    {
        if (effSlider == null)
            SetSliders();
        //
        effVolume = effSlider.value;
        SceneControl.instance.SaveOption_Sound();
        //
        Play(SoundKey.TEST, null, effSlider.value);
    }
    //==============================================//
    public void SetSoundAndSliderFromSaveData()
    {
        bgrVolume = (float)SceneControl.instance.saveData.bgrVolume;
        effVolume = (float)SceneControl.instance.saveData.effVolume;
        // Set Slider
        SetSliderValue();
    }
    //
    public void SetSliderValue()
    {
        if (bgrSlider == null || effSlider == null)
            SetSliders();
        //
        bgrSlider.value = bgrVolume;
        effSlider.value = effVolume;
    }
    //
    public void SetVolumeBySlider()
    { // Called By Slider
        if (bgrSlider == null || effSlider == null)
            SetSliders();
        //
        bgrVolume = bgrSlider.value;
        effVolume = effSlider.value;
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
        bgAuido.volume = bgrVolume;
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
            audio.volume = effVolume;
            audio.spatialBlend = 0.8f;

            fxAudios.Add(audio);

            obj.SetActive(false);
        }
    }
    //
    public void ResetBGRPitch()
    {
        if (bgAuido == null)
            return;
        //
        bgAuido.pitch = 1.0f;
    }
    //
    public void SetBGRPitch(int lv)
    {
        if (bgAuido == null)
            return;
        //
        float value = 1.0f + (float)lv * 0.2f;
        bgAuido.pitch = value;
    }
    //
    public void RePlayBGR()
    {
        StopBGR();
        PlayBGR(SoundKey.BGR);
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
    public void PlayBGR(SoundKey key, float vol = -1.0f)
    {
        if (clips.ContainsKey(key) == false)
            return;
        //
        bgAuido.clip = clips[key];
        if (vol == -1.0f)
            bgAuido.volume = bgrVolume;
        else
            bgAuido.volume = vol;
        //
        if(bgAuido.isPlaying == false)
            bgAuido.Play();
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
        if (transform == null)
            playAudio.transform.SetParent(Camera.main.transform);
        else
            playAudio.transform.SetParent(target);
        //
        playAudio.transform.localPosition = Vector3.zero;
        //
        if(effVol == -1.0f)
            playAudio.volume = effVolume;
        else
            playAudio.volume = effVol;
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
        playAudio.volume = effVolume;
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