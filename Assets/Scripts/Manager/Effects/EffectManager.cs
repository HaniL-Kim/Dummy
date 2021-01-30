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
    public List<string> effNames = new List<string>
    { "NONE", "Scan", "Explosion", "Flag"
    ,"Pull"};
    //==========================================//
    private void Awake()
    {
        instance = this;
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
        AddEffect(EffectType.SCAN);
        AddEffect(EffectType.EXPLOSION);
        AddEffect(EffectType.FLAG);
        AddEffect(EffectType.PULL);
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
            effect.SetActive(false);
            effect.name = effNames[(int)typeValue] + i;
            effect.GetComponent<SpriteEffect>().type = typeValue;
            //
            effects.Add(effect);
        }
        //
        totalEffect.Add(effNames[(int)typeValue], effects);
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
                Vector3 tempPos = tf.position;
                tempPos.z = -5.0f;
                effect.transform.position = tempPos;
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
