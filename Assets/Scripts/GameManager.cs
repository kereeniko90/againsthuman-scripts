using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelResourcesData levelResourcesData;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWinUI;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private AudioSource soundManager;
    [SerializeField] private AudioSource musicManager;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button winToMenuButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button defeatToMenuButton;
    [SerializeField] private Animator fadeAnimator;
    
    private GameObject canvas;
    private RectTransform lifeTransform;
    private RectTransform coinTranform;
    private Animator startButtonAnimator;
    private int levelIndex;

    private int currentCoin;
    private int currentLife;
    private int maxLife;
    private bool levelCompleted = false;

    private TextMeshProUGUI lifeText;
    private TextMeshProUGUI coinText;

    private bool levelWin = false;
    private bool levelLose = false;
    private float soundVolume;
    private float musicVolume;
    private float coinTimer = 0;
    

    private void Awake()
    {
        levelIndex = levelResourcesData.levelNumber;
        currentCoin = levelResourcesData.startingGold;
        currentLife = levelResourcesData.lifeCount;
        maxLife = currentLife;
        canvas = GameObject.Find("Canvas");
        soundVolume = PlayerPrefs.GetFloat("soundVolume", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 0.5f); 
    }

    private void Start()
    {   
        
        musicManager.volume = musicVolume;            
        soundManager.volume = soundVolume;
        lifeTransform = canvas.transform.Find("Status_Life").GetComponent<RectTransform>();
        lifeText = lifeTransform.GetComponentInChildren<TextMeshProUGUI>();
        lifeText.text = currentLife + "/" + maxLife;

        coinTranform = canvas.transform.Find("Status_Coin").GetComponent<RectTransform>();
        coinText = coinTranform.GetComponentInChildren<TextMeshProUGUI>();
        coinText.text = currentCoin.ToString();
        startButtonAnimator = startButton.GetComponent<Animator>();

        gameOverUI.SetActive(false);
        startButton.gameObject.SetActive(true);
        startButtonAnimator.SetBool("show", true);
        backToMenuButton.onClick.AddListener(LoadMainScreen);
        winToMenuButton.onClick.AddListener(LoadMainScreen);
        nextLevelButton.onClick.AddListener(LoadNextScene);
        defeatToMenuButton.onClick.AddListener(LoadMainScreen);
        retryButton.onClick.AddListener(RetryScene);

        if (levelIndex >= 3) {
            nextLevelButton.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (currentLife == 0)
        {
            gameOverUI.SetActive(true);

            if (!levelLose) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Defeat);
                levelLose = true;
            }
        }

        if (enemySpawner.IsWaveStarted())
        {


            startButton.interactable = false;
            startButtonAnimator.SetBool("show", false);


        }
        else if (enemySpawner.GetActiveEnemyCount() == 0)
        {
            startButton.interactable = true;
            startButtonAnimator.SetBool("show", true);
        }

        waveText.text = enemySpawner.GetCurrentWave();

        if (levelCompleted && currentLife > 0)
        {
            StartCoroutine(LevelWin());
        }

        if (enemySpawner.IsWaveStarted()) {
            CoinIncrement();
        }
        


    }

    private IEnumerator LevelWin()
    {
        yield return new WaitForSeconds(1f);
        gameWinUI.SetActive(true);
        if (!levelWin) {
                SoundManager.Instance.PlaySound(SoundManager.Sound.Win);

                int currentLevel = PlayerPrefs.GetInt("unlockedLevels", 1);

                if (currentLevel < levelIndex + 1) {
                    PlayerPrefs.SetInt("unlockedLevels", levelIndex + 1);
                }
                
                levelWin = true;
            }
    }

    public void ReduceLife(EnemyStatus enemyStatus)
    {
        if (currentLife > 0)
        {

            if (enemyStatus.enemy.enemyType == Enemy.EnemyType.Normal)
            {
                currentLife -= 1;
            }
            else if (enemyStatus.enemy.enemyType == Enemy.EnemyType.Boss)
            {
                currentLife = 0;
            }

        }

        lifeText.text = currentLife + "/" + maxLife;
    }

    public void ReduceCoin(int cost)
    {
        currentCoin -= cost;
        coinText.text = currentCoin.ToString();
    }

    public void AddCoin(int coin)
    {
        currentCoin += coin;
        coinText.text = currentCoin.ToString();
    }

    public int GetCurrentCoin()
    {
        return currentCoin;
    }

    public void LevelCompleted()
    {
        levelCompleted = true;
    }

    public void HideStartButton()
    {
        startButton.gameObject.SetActive(false);
    }

    public void PlayDefeatSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Defeat);
    }

    public void PlayWinSound() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Win);
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
    }

    public void StartScene(int sceneNumber)
    {
        StartCoroutine(PlayFadeOutAndLoadScene(sceneNumber));
    }

    public void LoadMainScreen() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        StartScene(0);
    }

    public void LoadNextScene() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        StartScene(levelIndex + 1);
    }

    public void RetryScene() {
        SoundManager.Instance.PlaySound(SoundManager.Sound.InteractableButton);
        StartScene(levelIndex);
    }

    private void CoinIncrement() {
        coinTimer += Time.deltaTime;

        if (coinTimer >= 1) {
            currentCoin += 1;
            coinText.text = currentCoin.ToString();
            coinTimer = 0;
        }
    }



}
