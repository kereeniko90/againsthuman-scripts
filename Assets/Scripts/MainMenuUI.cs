using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject startTitle;
    [SerializeField] private GameObject startGame;
    [SerializeField] private GameObject menuBackground;
    [SerializeField] private GameObject menuButton;
    [SerializeField] private GameObject settingWindow;
    [SerializeField] private GameObject levelsWindow;
    [SerializeField] private Button levelButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button startScreenButton;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button settingWindowClose;
    [SerializeField] private Button closeLevelButton;
    [SerializeField] private AudioSource musicManagerAudioSource;
    [SerializeField] private AudioSource soundManagerAudioSource;
    [SerializeField] private GameObject levelList;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    [SerializeField] private Animator fadeAnimator;

    private Animator startTitleAnimator;
    private Animator startGameAnimator;
    private Animator menuBackgroundAnimator;
    private Animator menuButtonAnimator;
    private Animator settingWindowAnimator;
    private Animator levelsWindowAnimator;
    private float soundVolume;
    private float musicVolume;
    private bool settingWindowOpen = false;
    private int unlockedLevels;

    private void Awake()
    {
        soundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 1);
    }

    private void Start()
    {
        startTitleAnimator = startTitle.GetComponent<Animator>();
        startGameAnimator = startGame.GetComponent<Animator>();
        menuBackgroundAnimator = menuBackground.GetComponent<Animator>();
        settingWindowAnimator = settingWindow.GetComponent<Animator>();
        levelsWindowAnimator = levelsWindow.GetComponent<Animator>();

        menuButtonAnimator = menuButton.GetComponent<Animator>();
        menuBackground.SetActive(false);
        menuButton.SetActive(false);
        startTitleAnimator.SetBool("show", true);
        startGameAnimator.SetBool("show", true);

        startScreenButton.onClick.AddListener(ShowStartScreen);
        settingWindowClose.onClick.AddListener(CloseSettingWindow);
        settingButton.onClick.AddListener(ShowSettings);
        levelButton.onClick.AddListener(ShowLevels);
        closeLevelButton.onClick.AddListener(CloseLevels);
        level1Button.onClick.AddListener(LoadLevelOne);
        level2Button.onClick.AddListener(LoadLevelTwo);
        level3Button.onClick.AddListener(LoadLevelThree);

        soundSlider.value = soundVolume;
        musicSlider.value = musicVolume;
        musicManagerAudioSource.volume = musicVolume;
        soundManagerAudioSource.volume = soundVolume;
        CheckUnlockedLevels();

    }

    private void Update()
    {
        if (settingWindowOpen)
        {
            musicManagerAudioSource.volume = musicSlider.value;
            musicVolume = musicManagerAudioSource.volume;
            soundManagerAudioSource.volume = soundSlider.value;
            soundVolume = soundManagerAudioSource.volume;

        }
    }


    public void StartGame()
    {

        menuBackground.SetActive(true);
        menuBackgroundAnimator.SetBool("show", true);
        startTitleAnimator.SetBool("show", false);
        startGameAnimator.SetBool("show", false);
        menuButton.SetActive(true);
        menuButtonAnimator.SetBool("show", true);
    }

    private void ShowStartScreen()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        menuBackgroundAnimator.SetBool("show", false);
        startTitleAnimator.SetBool("show", true);
        startGameAnimator.SetBool("show", true);
        menuButtonAnimator.SetBool("show", false);
    }

    private void ShowSettings()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        menuButtonAnimator.SetBool("show", false);
        settingWindowOpen = true;
        settingWindowAnimator.SetBool("show", true);

    }

    private void ShowLevels()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        menuButtonAnimator.SetBool("show", false);

        levelsWindowAnimator.SetBool("show", true);
    }

    private void CloseLevels()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        menuButtonAnimator.SetBool("show", true);

        levelsWindowAnimator.SetBool("show", false);
    }

    public void HideBackground()
    {
        menuBackground.SetActive(false);
    }

    public void DisableMenuButton()
    {
        menuButton.SetActive(false);

    }

    public void CloseSettingWindow()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        settingWindowAnimator.SetBool("show", false);
        menuButtonAnimator.SetBool("show", true);
        PlayerPrefs.SetFloat("soundVolume", soundVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        settingWindowOpen = false;
    }

    private void CheckUnlockedLevels()
    {

        for (int i = 0; i < levelList.transform.childCount; i++)
        {
            Transform level = levelList.transform.GetChild(i);
            GameObject lockFrame = level.Find("LockFrame").gameObject;
            GameObject stageComplete = level.Find("Stage_Complete").gameObject;

            if (i < unlockedLevels)
            {
                stageComplete.SetActive(true);


                lockFrame.SetActive(false);
            }
            else
            {
                stageComplete.SetActive(false);
                lockFrame.SetActive(true);
            }
        }
    }

    public void StartScene(int sceneNumber)
    {
        //StartCoroutine(LoadNewScene(sceneNumber));
        StartCoroutine(PlayFadeOutAndLoadScene(sceneNumber));
    }

    IEnumerator LoadAsyncGame(int sceneNumber)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNumber);

        while (!operation.isDone)
        {

            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //loadingSlider.value = progress;

            yield return null;
        }

        fadeAnimator.SetTrigger("FadeIn");

    }

    private IEnumerator PlayFadeOutAndLoadScene(int sceneNumber)
    {

        fadeAnimator.SetTrigger("FadeOut");


        yield return new WaitForSeconds(fadeAnimator.GetCurrentAnimatorStateInfo(0).length);


        yield return StartCoroutine(LoadAsyncGame(sceneNumber));
        //SceneManager.LoadScene(sceneNumber);
    }

    private IEnumerator LoadNewScene(int sceneNumber)
    {
        
        fadeAnimator.SetTrigger("FadeOut");

       
        yield return new WaitForSeconds(fadeAnimator.GetCurrentAnimatorStateInfo(0).length);

        
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);
        asyncLoad.allowSceneActivation = false;

        
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        
        fadeAnimator.SetTrigger("FadeIn");
    }

    public void LoadLevelOne()
    {
        StartScene(1);
    }

    public void LoadLevelTwo()
    {
        StartScene(2);
    }

    public void LoadLevelThree()
    {
        StartScene(3);
    }


}
