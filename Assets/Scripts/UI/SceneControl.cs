using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public int currentBGR;
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
            saveData = new SaveData();
            Load();
            //
            SceneManager.activeSceneChanged += OnSceneChange;
        }
        else
            Destroy(this.gameObject);
    }
    //==================================//
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
    private void OnSceneChange(Scene current, Scene next)
    {
        //if (next.buildIndex == 2)
        if (next.name == "2_StageSelectScene")
        {
            SetStageBtns(false);
            //
            swc = FindObjectOfType<StageWindowControl>(true);
            swc.CheckBGREnabled();
            swc.CheckBGR(currentBGR);
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
            for (int i = 0; i < 3; ++i)
                stages[i] = bm.stagePanels[i].transform.GetChild(1).gameObject;
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
