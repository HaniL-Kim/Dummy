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
    //public GameObject[] stages;
    // ================= UI State ================= //
    public Difficulty currentDifficulty = Difficulty.NORMAL;
    public int currentStage = 1;
    //
    public int currentBGR;
    // ================= UI ================= //
    public Canvas canvas;
    public Camera cam;
    public InGameMenuControl IGMControl;
    public OptionManager om;
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
        //
        Load();
        //
        om.SetScreenAndCheckBoxFromSaveData();
        //
        SceneManager.activeSceneChanged += OnSceneChange;
    }
    //==================================//
    public void SaveRecord()
    {
        string record = GameManager.instance.elecShooter.superViser.currentFloor.ToString();
        saveData.bestRecord[(int)currentDifficulty] = record;
    }
    //
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
    public IEnumerator ClearSequence()
    {
        /*
        DOTween.KillAll();
        //
        Debug.Log("Begin Clear Sequence");
        Time.timeScale = 0.0f;
        //
        SaveClearData();
        */
        Debug.Log("Begin Clear Sequence");
        Sequence ClearSequence = DOTween.Sequence()
            .AppendCallback(() => { StartSceneTransition("2_StageSelectScene"); })
            // after finish load stageSelectScene
            .AppendInterval(2.0f)
            .AppendCallback(() => { SetStageBtns(true); })
            .SetUpdate(true);
        // 
        Time.timeScale = 1.0f;
        Debug.Log("End Clear Sequence");
        yield return null;
    }
    //
    public IEnumerator LoadSceneAsyncWithTransitionCoroutine(string sceneName)
    {
        //Debug.Log("Load " + sceneName + " begin");
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
                if (op.isDone == true)
                    TransitionControl.instance.Door_Open(sceneName);
            }
        }
        //
        //Debug.Log("Load " + sceneName + " complete");
    }
    //
    public void SetIGM(bool b)
    {
        IGMControl.SetIGMControl(b);
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
        //cam.GetComponent<AudioListener>().enabled = b;
    }
    //
    private void OnSceneChange(Scene current, Scene next)
    {
        if (next.name != "MainScene")
        {
            foreach (var canvas in FindObjectsOfType<Canvas>())
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
        //else if (next.name == "MainScene")
        //{
        //    // cam.enabled = false;
        //}
        else
        {
            bm = null;
            swc = null;
            //stages = null;
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
        if(bm == null)
        {
            bm = FindObjectOfType<ButtonManager>(true);
            bm.SetPanel(currentDifficulty);
        }
        //
        { // 0 : locked, 1 : unlock Effect, 2 : unlocked, 3 : clear
            for(int i = 0; i < bm.stagePanels.Count; ++i)
            {
                // Child[0] : Panel_DifficultyText, Child[1] : STAGE
                Transform stage = bm.stagePanels[i].GetChild(1);
                // Activate first stage
                StageButton first = stage.GetChild(1).GetComponentInChildren<StageButton>();
                first.Activate(false);
                // stageBtn children order : 0(inf), 1(first), 2 ~ 6
                for (int j = 0; j < stage.childCount; ++j)
                {
                    int btnState = saveData.stageBtnState[i][j];
                    StageButton sb = stage.GetChild(j).GetComponentInChildren<StageButton>();
                    if(tween == false)
                    { // call by SceneChange
                        switch (btnState)
                        {
                            case 0: // locked
                            case 1: // unlock Effect
                                break;
                            case 2: // unlocked
                            case 3: // clear
                                sb.Activate(false);
                                break;
                        }
                    }
                    else
                    { // call by Clear Sequence
                        switch (btnState)
                        {
                            case 0: // locked
                                break;
                            case 1: // unlock Effect
                                sb.Activate(true);
                                saveData.stageBtnState[i][j] = 2;
                                Save();
                                break;
                            case 2: // unlocked
                            case 3: // clear
                                break;
                        }

                    }
                }

            }

        }
    }
    //
    public void Load()
    {
        ReadStageTable();
        saveData = MyUtility.LoadDataFromJson();
        //
        Debug.Log("Load Complete");
    }
    //
    public void SaveClearData()
    {
        for (int i = (int)currentDifficulty; i >= 0; --i)
        {
            saveData.stageBtnState[i][currentStage] = 3;
            //
            int nextStageIndex = 0;
            if (currentStage == 0)
            { // current : 0 stage(infinite)
                // Do Nothing
            }
            else if (currentStage == saveData.stageBtnState[i].intArr.Length - 1)
            { // current : 6 stage
                nextStageIndex = 0;
                // Ending Sequence?
            }
            else
            { // current : 1 ~ 5 stage
                nextStageIndex = currentStage + 1;
            }
            //
            int nextStageState = saveData.stageBtnState[i][nextStageIndex];
            if (nextStageState == 0)
                saveData.stageBtnState[i][nextStageIndex] = 1;
        }
        //
        Save();
    }
    //
    public void SaveOption_Screen()
    {
        saveData.resolution = (int)om.curResolution;
        saveData.screenMode = (int)om.curScreenMode;
        Save();
    }
    //
    public void SaveOption_Sound()
    {
        saveData.bgrVolume = SoundManager.instance.bgrVolume;
        saveData.effVolume = SoundManager.instance.effVolume;
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
