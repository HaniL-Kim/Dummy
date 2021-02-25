using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    //==========================================//
    public static EffectManager instance;
    //==========================================//
    // map
    private Dictionary<string, List<GameObject>> totalEffect =
        new Dictionary<string, List<GameObject>>();
    //==========================================//
    public GameObject bloom;
    public VolumeProfile bloomVp;
    //==========================================//
    public Dictionary<int, Transform> effHolders = new Dictionary<int, Transform>();
    public List<string> effNames = new List<string>();
    //==========================================//
    private void Awake()
    {
        instance = this;
        //
        effNames = new List<string>
        { "NONE", "Scan", "Explosion", "Flag", "Pull", "Push", "Thunder" };
    }
    //
    private void Start()
    {
        bloom = GameManager.instance.transform.GetChild(0).gameObject;
        bloomVp = bloom.GetComponent<Volume>().profile;
    }
    //==========================================//
    public void CreateEffect()
    {
        for(int i = 1; i < (int)EffectType.END; ++i)
            AddEffect((EffectType)i);
    }
    private void AddEffect(EffectType typeValue, int poolCount = 30)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Effects/" + effNames[(int)typeValue]);
        //
        List<GameObject> effects = new List<GameObject>();
        //
        GameObject holder = new GameObject(effNames[(int)typeValue] + "_Holder");
        holder.transform.SetParent(transform);
        //
        effHolders.Add((int)typeValue, holder.transform);
        //
        for (int i = 0; i < poolCount; ++i)
        {
            GameObject effect = Instantiate(prefab, holder.transform);
            effect.name = effNames[(int)typeValue] + i;
            effect.SetActive(false);
            //
            SpriteEffect se = null;
            if(effect.TryGetComponent<SpriteEffect>(out se))
                se.type = typeValue;
            //effect.GetComponent<SpriteEffect>().type = typeValue;
            //
            effects.Add(effect);
        }
        //
        totalEffect.Add(effNames[(int)typeValue], effects);
    }
    //
    public void DownActiveEffectHeight(string key, float value)
    {
        if (totalEffect.ContainsKey(key) == false)
            return;
        //
        List<GameObject> effects = totalEffect[key];
        //
        foreach (GameObject effect in effects)
        {
            if (effect.activeSelf == false)
                return;
            //
            MyUtility.DownHeight(effect.transform, value);
        }
    }
    //
    public void Play(string key, Transform tf)
    {
        if (totalEffect.ContainsKey(key) == false)
            return;
        //
        List<GameObject> effects = totalEffect[key];
        //
        foreach (GameObject effect in effects)
        {
            if (effect.activeSelf == false)
            {
                effect.SetActive(true);
                effect.transform.SetParent(tf);
                effect.transform.localPosition = Vector3.zero;
                return;
            }
        }
    }
    //
    public void Play(string key, Vector3 pos, Quaternion rot)
    {
        if (totalEffect.ContainsKey(key) == false)
            return;
        //
        List<GameObject> effects = totalEffect[key];
        //
        foreach(GameObject effect in effects)
        {
            if(effect.activeSelf == false)
            {
                effect.SetActive(true);
                effect.transform.position = pos;
                effect.transform.rotation = rot;
                //
                return;
            }
        }
    }
    //
    public void SetBloom(bool value)
    {
        Bloom bloom;
        bloomVp.TryGet(out bloom);
        bloom.active = value;
        //bloom.intensity.value = 0.5f;
        //bloom.intensity.Override(0.5f);
    }
    //
    public void Fire(string key, Transform start, Transform end, float arriveTime)
    {
        if (totalEffect.ContainsKey(key) == false)
            return;
        //
        List<GameObject> effects = totalEffect[key];
        //
        foreach(GameObject effect in effects)
        {
            if(effect.activeSelf == false)
            {
                effect.SetActive(true);
                effect.GetComponent<FlagEffect>().Set(start, end, arriveTime);
                //
                return;
            }
        }
    }
}
