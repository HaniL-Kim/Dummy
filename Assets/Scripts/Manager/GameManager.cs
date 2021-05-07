using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//==================// //==================//
public enum MineTypes
{
    NONE = -1,
    PULL, PUSH, GHOST, THUNDER, NARROWING, CRASH
}
//
public enum NumberIcons
{
    ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX,
}
//==================// //==================//
public class GameManager : MonoBehaviour
{
    //====================================//
    public static GameManager instance;
    //====================================//
    public bool stageClear = false;
    public bool gameOver = false;
    public bool pause = false;
    //====================================//
    public Dummy dummy;
    public ElecShooterController elecShooter;
    //====================================//
    // layer hashing
    [HideInInspector] public int footBoardLayer;
    [HideInInspector] public int concreteLayer;
    [HideInInspector] public int concreteBoxLayer;
    [HideInInspector] public int closetLayer;
    [HideInInspector] public int innerLayer;
    //====================================//
    public List<Vector3> dirs = new List<Vector3>();
    //====================================//
    KeyCode[] controlKeyCodes = new KeyCode[] {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
        KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Space
    };
    //====================================//
    public int mineCount = 0;
    //====================================// //====================================//
    private void Awake()
    {
        instance = this;
        //
        CreateLayerHash();
        SetDirs();
    }
    //
    private void Start()
    {
        EffectManager.instance.CreateEffect();
        SoundManager.instance.RePlayBGR();
    }
    //====================================//
    private void Update()
    {
        LetElecShooterMove();
    }
    //====================================//
    public RaycastHit2D RayToDirs(Transform tf, int dir, int layer)
    {
        float dist = dirs[dir].magnitude;
        RaycastHit2D hit = Physics2D.Raycast(tf.position, dirs[dir], dist, layer);
        //Debug.DrawRay(tf.position, dirs[dir], Color.green, 5.0f);
        return hit;
    }
    //====================================//
    public void Pause(bool b)
    {
        Time.timeScale = b ? 0.0f : 1.0f;
        pause = b;
    }
    //
    public void ClearStage()
    {
        if (stageClear == true)
            return;
        //
        stageClear = true;
        //
        dummy.isInvincible = true;
        //
        MineController.instance.StopAllGhost();
        //
        SoundManager.instance.StopBGR();
        //Debug.Log("Begin ClearSTage");
        // Pause(true);
        /* Pause
        0. Stop Dummy Control
        0. Stop ElectShooter
        0. Stop All Mine(Ghost, Thunder)
        */
        // Scene Transition
        StartCoroutine(SceneControl.instance.ClearSequence());
        
        //Debug.Log("End ClearStage");
    }
    //====================================//
    private void LetElecShooterMove()
    {
        if (dummy.isDead == false)
            if (elecShooter.isElevate == false)
                for (int i = 0; i < controlKeyCodes.Length; ++i)
                    if (Input.GetKeyDown(controlKeyCodes[i]))
                        elecShooter.isElevate = true;
    }
    //
    public void StartGame()
    {
        dummy.gameObject.SetActive(true);
    }
    //====================================//
    private void SetDirs()
    {
        dirs.Add(new Vector3(+64.0f, 0, 4.0f));
        dirs.Add(new Vector3(+32.0f, +47.0f, 4.0f));
        dirs.Add(new Vector3(-32.0f, +47.0f, 4.0f));
        dirs.Add(new Vector3(-64.0f, 0, 4.0f));
        dirs.Add(new Vector3(-32.0f, -47.0f, 4.0f));
        dirs.Add(new Vector3(+32.0f, -47.0f, 4.0f));
    }
    //
    private void CreateLayerHash()
    {
        footBoardLayer = LayerMask.GetMask("FootBoard");
        concreteLayer = LayerMask.GetMask("Concrete");
        concreteBoxLayer = LayerMask.GetMask("ConcreteBox");
        closetLayer = LayerMask.GetMask("Closet");
        innerLayer = LayerMask.GetMask("Inner");
    }
    //
    public Transform GetTileTransformOnMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dirMouseToForward = new Vector3(mousePos.x, mousePos.y, 20.0f);
        //
        RaycastHit2D hit = Physics2D.Raycast(
            mousePos, dirMouseToForward, 1.0f, closetLayer);
        //
        if (hit)
            return hit.transform;
        else
            return null;
    }
    //====================================//
    public void ReloadPlayScene()
    {
        elecShooter.isElevate = false;
        //
        StartCoroutine(RetryMainScene("MainScene"));
    }
    //
    private IEnumerator RetryMainScene(string sceneName)
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(sceneName);
        asyncOper.allowSceneActivation = false;
        //
        UIManager.instance.ActivatePressToRetry();
        //
        while (!asyncOper.isDone)
        {
            //UIManager.instance.SetSliderValue(asyncOper.progress);
            if (asyncOper.progress >= 0.9f)
            {
                if (Input.anyKeyDown)
                {
                    yield return new WaitForSeconds(1.0f);
                    asyncOper.allowSceneActivation = true;
                }
            }
            //
            yield return null;
        }
    }
}