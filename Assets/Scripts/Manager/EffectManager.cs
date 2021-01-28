using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectManager : MonoBehaviour
{
    //==========================================//
    // Singleton
    static private EffectManager _instance;
    static public EffectManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("EffectManager");
                _instance = obj.AddComponent<EffectManager>();
            }
            return _instance;
        }
    }
    //==========================================//
    // map
    private Dictionary<string, List<GameObject>> totalEffect =
        new Dictionary<string, List<GameObject>>();
    //==========================================//
    public GameObject bloom;
    public VolumeProfile bloomVp;
    //==========================================//
    public Transform scanHolder;
    public Transform flagHolder;
    //==========================================//
    private void Start()
    {
        bloom = GameManager.instance.transform.GetChild(0).gameObject;
        bloomVp = bloom.GetComponent<Volume>().profile;
    }
    //==========================================//
    public void CreateEffect()
    {
        AddEffect("Scan");
        AddEffect("Flag");
    }
    private void AddEffect(string name, int poolCount = 30)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Effects/" + name);
        //
        List<GameObject> effects = new List<GameObject>();
        //
        GameObject holder = new GameObject(name + "_Holder");
        holder.transform.SetParent(transform);
        //
        if (name == "Scan")
            scanHolder = holder.transform;
        if (name == "Flag")
            flagHolder = holder.transform;
        //
        for (int i = 0; i < poolCount; ++i)
        {
            GameObject effect = Instantiate(prefab, holder.transform);
            effect.SetActive(false);
            effect.name = name + i;
            //
            effects.Add(effect);
        }
        //
        totalEffect.Add(name, effects);
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
                effect.transform.position = tf.position;
                effect.transform.localRotation = Quaternion.identity;
                //
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
