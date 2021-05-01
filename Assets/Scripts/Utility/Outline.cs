using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    //=============================================================//
    public Material outlineMaterial;
    //=============================================================//
    private SpriteRenderer sr;
    //
    public GameObject outline;
    private SpriteRenderer outline_sr;
    private Material outline_mr;
    // shader Hash
    private readonly int hashOutlineColorInner = Shader.PropertyToID("_OutlineColor_Inner");
    private readonly int hashOutlineTK = Shader.PropertyToID("_OutlineThickness");
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
    public void SetOutline(Color c, float f)
    {
        outline_mr.SetColor(hashOutlineColorInner, c);
        outline_mr.SetFloat(hashOutlineTK, f);
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
        outline_sr.sortingLayerName = "AirJumpPack";
        outline_sr.sortingOrder = -10;
        outline_mr = outline_sr.material;
    }
    //=============================================================//
}
