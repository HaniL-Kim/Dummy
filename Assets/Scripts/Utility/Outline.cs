using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Outline : MonoBehaviour
{
    //=============================================================//
    public Material outlineMaterial;
    //=============================================================//
    private SpriteRenderer sr;
    //
    public GameObject outline;
    public SpriteRenderer outline_sr;
    private Material outline_mat;
    // shader Hash
    private readonly int hashOutlineColorInner = Shader.PropertyToID("_OutlineColor_Inner");
    private readonly int hashOutlineTK = Shader.PropertyToID("_OutlineThickness");
    //
    public string outlineSortingLayerName;
    //=============================================================//
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        AddOutline(sr);
    }
    //
    private void Update()
    {
        outline_sr.sprite = sr.sprite;
        outline_sr.flipX = sr.flipX;
    }
    //=============================================================//
    public void FadeOutline(Color c, float f)
    {
        outline_sr.DOColor(c, f);
    }
    //
    public void SetOutline(Color c, float f)
    {
        outline_sr.color = Color.white;
        //
        outline_mat.SetColor(hashOutlineColorInner, c);
        outline_mat.SetFloat(hashOutlineTK, f);
    }
    //=============================================================//
    private void AddOutline(SpriteRenderer sr)
    {
        outline = new GameObject("Outline");
        outline.transform.parent = sr.gameObject.transform;
        outline.transform.localScale = new Vector3(1f, 1f, 1f);
        outline.transform.localPosition = new Vector3(0f, 0f, 0f);
        outline.transform.localRotation = Quaternion.identity;
        //
        outline_sr = outline.AddComponent<SpriteRenderer>();
        outline_sr.sprite = sr.sprite;
        outline_sr.material = outlineMaterial;
        //
        if(outlineSortingLayerName == "AirJumpPack")
        {
            outline_sr.sortingLayerName = "AirJumpPack";
            outline_sr.sortingOrder = -10;
        }
        else
        {
            outline_sr.sortingLayerName = sr.sortingLayerName;
            outline_sr.sortingOrder = sr.sortingOrder;
        }
        //
        outline_mat = outline_sr.material;
    }
    //=============================================================//
}
