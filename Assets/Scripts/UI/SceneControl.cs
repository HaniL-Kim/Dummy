using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//
using DG.Tweening;
// namespace //
using MyUtilityNS;
//
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneControl : MonoBehaviour
{
    // ================= Singleton ================= //
    public static SceneControl instance;
    // ================= Stage Window ================= //
    public StageWindowControl swc;
    public ButtonManager bm;
    // ================= StageBtn panels ================= //
    public GameObject[] stages;
    // ================= UI State ================= //
    public Difficulty currentDifficulty = Difficulty.NORMAL;
    public int currentStage = 1;
    //
    public int currentBGR;
    // ================= UI ================= //
    public Canvas canvas;
    public Camera cam;
    public InGameMenuControl IGMControl;
    public GameObject option;
    // ================= Read from csv & json ================= //
    public DicStageData stageData;
    public SaveData saveData;
    //==================================//
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            //
            Init();
        }
        else
            Destroy(this.gameObject);
    }
    //==================================//
    private void Init()
    {
        //canvas = transform.GetChild(0).GetComponent<Canvas>();
        canvas.worldCamera = cam;
        //
        saveData = new SaveData();
        Load();
        //
        SceneManager.activeSceneChanged += OnSceneChange;
    }
    //==================================//
    public void StartInforScene()
    {
        StartCoroutine(InforCoroutine());
    }
    //
    public IEnumerator InforCoroutine()
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        //
        AsyncOperation op = SceneManager.LoadSceneAsync("3_InforScene", LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        while(op.isDone == false)
        {
            yield return null;
            if(op.progress >= 0.9f)
            {
                //op.allowSceneActivation = true;
                //
                if (op.isDone == false)
                {
                    op.allowSceneActivation = true;
                }
                else
                {
                    Scene s = SceneManager.GetSceneAt(1);
                    SceneManager.SetActiveScene(s);
                }
            }
        }
    }
    //
    public void StartSceneTransition(string sceneName)
    {
        // Transition Sequence
        Sequence ActivateSequence = DOTween.Sequence()
        .AppendCallback(() => { TransitionControl.instance.Door_Close(); })
        .AppendInterval(TransitionControl.instance.tweenTime)
        .AppendCallback(() => { StartSceneLoadingWithTransition(sceneName); })
        .SetUpdate(true);
        ;
    }
    //
    public void StartSceneLoadingWithTransition(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncWithTransitionCoroutine(sceneName));
    }
    //
    //public void StartMainScene()
    //{
    //    StartCoroutine(LoadSceneAsyncWithTransitionCoroutine("MainScene"));
    //}
    //
    public IEnumerator LoadSceneAsyncWithTransitionCoroutine(string sceneName)
    {
        Debug.Log("Load " + sceneName + " begin");
        //
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while(op.isDone == false)
        {
            yield return null;
            if(op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
                //
                if(op.isDone == true)
                    TransitionControl.instance.Door_Open(sceneName);
            }
        }
        //
        Debug.Log("Load " + sceneName + " complete");
    }
    //
    public void SetScreen(int res = -1, int mod = -1)
    {
        // Default State
        Resolution curResolution = Resolution.FHD;
        ScreenMode curScreenMode = ScreenMode.BLFS;
        //
        if(res == -1 || mod == -1)
        {
            curResolution = (Resolution)saveData.resolution;
            curScreenMode = (ScreenMode)saveData.screenMode;
        }
        else
        {
            curResolution = (Resolution)res;
            curScreenMode = (ScreenMode)mod;
        }
        //
        FullScreenMode fsMod = FullScreenMode.ExclusiveFullScreen;
        //
        switch (curScreenMode)
        {
            case ScreenMode.NONE:
                fsMod = FullScreenMode.Windowed;
                break;
            case ScreenMode.FS:
                fsMod = FullScreenMode.ExclusiveFullScreen;
                break;
            case ScreenMode.BLFS:
                fsMod = FullScreenMode.FullScreenWindow;
                break;
            default:
                break;
        }
        //
        switch (curResolution)
        {
            case Resolution.FHD:
                Screen.SetResolution(1920, 1080, fsMod);
                break;
            case Resolution.HD:
                Screen.SetResolution(1280, 720, fsMod);
                break;
            default:
                break;
        }
    }
    //
    public void SetIGM(bool b)
    {
        IGMControl.Set(b);
        //
        cam.enabled = b;
    }
    //
    public bool IGMEnabled()
    {
        return IGMControl.inGameMenu.activeSelf;
    }
    //
    public void UICamSet(bool b)
    {
        cam.enabled = b;
        cam.GetComponent<AudioListener>().enabled = b;
    }
    //
    private void OnSceneChange(Scene current, Scene next)
    {
        foreach (var canvas in FindObjectsOfType<Canvas>())
        {
            canvas.worldCamera = cam;
        }
        
        //
        if (next.name == "2_StageSelectScene")
        {
            SetStageBtns(false);
            //
            swc = FindObjectOfType<StageWindowControl>(true);
            swc.CheckBGREnabled();
            swc.CheckBGR(currentBGR);
        }
        else if (next.name == "MainScene")
        {
            // cam.enabled = false;
        }
        else
        {
            bm = null;
            swc = null;
            stages = null;
        }
    }
    //
    public void ReadStageTable()
    {
        stageData = MyUtility.ReadStageData();
    }
    //
    private void SetStageBtns(bool tween = true)
    {
        stages = GameObject.FindGameObjectsWithTag("Stage");
        if (stages == null)
            ExitGame();
        //
        if(bm == null)
        {
            bm = FindObjectOfType<ButtonManager>(true);
            bm.SetPanel(currentDifficulty);
            //
            //for (int i = 0; i < 3; ++i)
            //    stages[i] = bm.stagePanels[i].transform.GetChild(1).gameObject;
        }
        //
        for (int i = 0; i < stages.Length; ++i)
        {
            // open 1 stage
            stages[i].transform.GetChild(0).GetComponentInChildren<StageButton>().Activate(false);
            //
            int clearStageMax = saveData.stageClear[i];
            for (int j = 0; j < clearStageMax; ++j)
            {
                stages[i].transform.GetChild(j + 1).GetComponentInChildren<StageButton>().Activate(false);
            }

        }

    }
    //
    public void Load()
    {
        ReadStageTable();
        saveData = MyUtility.LoadDataFromJson();
        //
        SetScreen();
        //
        Debug.Log("Load Complete");
    }
    public void SaveOption(int res, int mod, double bgVol, double effVol)
    {
        saveData.resolution = res;
        saveData.screenMode = mod;
        saveData.bgrVolume = bgVol;
        saveData.effVolume = effVol;
        Save();
    }
    //
    public void Save()
    {
        MyUtility.SaveDataToJson(saveData);
        Debug.Log("Save Complete");
    }
    //
    public void OpenOption(bool b)
    {
        option.SetActive(b);
    }
    //==================================//
    public void ExitGame()
    { // Button Event
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL("http://google.com");
#else
        Application.Quit(); //어플리케이션 종료
#endif
    }
    // ================= Custom Editor ================= //
#if UNITY_EDITOR
    [CustomEditor(typeof(SceneControl))]
    public class ActivateButton : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //
            SceneControl sc = (SceneControl)target;
            //
            if (GUILayout.Button("Save"))
                sc.Save();
            if (GUILayout.Button("Load"))
                sc.Load();
        }
    }
#endif
}
