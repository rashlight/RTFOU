using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalProc : MonoBehaviour
{
    public int playerHealthBonus = 2;
    public float playerBonusThreshold = 1f;
    /// <summary>
    /// Reference to GlobalProc (this). Use with extreme caution.
    /// </summary>
    public static GlobalProc self;

    private int resetAllConfirm = 11;
    [SerializeField] private float gameTimeDelay;
    [SerializeField] private float currentTimeScale;
    [SerializeField] private Text timeText;
    [SerializeField] private Text seedText;
    public Text gameMultiplierText;
    [SerializeField] private Player player;
    [SerializeField] private Canvas globalCanvas;
    public GameObject pausePanel;
    [SerializeField] private Text debugText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text highscoreText;
    [SerializeField] private InputField seedField;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Wilberforce.FinalVignette.FinalVignette vignetteManager;
    [SerializeField] private AudioSource playerDeathSound;
    [SerializeField] private Gun[] gunList;

    private float slowFramesTime = 0f;
    private ulong slowframeCount = 0;
    private float gameTimeStamp = 0f;
    private float gameTimeDelayTimer = 0f;
    private string sessionVerb;

    private const string DEBUG_GUID = "debug";
    private const string BOSSRUSH_GUID = "bossrush";
    private const string LOWLUXBACKGROUND_GUID = "lowluxbackground";

    public void ExitGame()
    {
        Application.Quit();
    }

    #region UI Actions
    public void ResetConfirmTime(Text resetText)
    {
        resetAllConfirm = 11;
        resetText.text = "ERASE EVERYTHING?";
    }
    public void ResetAll(Text resetText)
    {
        resetAllConfirm--;
        if (resetAllConfirm < 0)
        {
            resetText.text = "Data deleted.";
            PlayerPrefs.DeleteAll();
        } 
        else 
        {      
            resetText.text = "Are you sure? (" + resetAllConfirm + ")";         
        }
    }

    public void ResetTimescale()
    {
        Time.timeScale = 1.0f;
    }
    public void ToggleShaderStatus(Text shaderStatusText)
    {
        SetShaderState(!LoadShaderState());
        InitShaderStatus(shaderStatusText);
        print(LoadShaderState());
    }
    public void InitShaderStatus(Text shaderStatusText)
    {
        bool status = LoadShaderState();
        shaderStatusText.text = "SHADER:\n" + ((status) ? "ENABLED" : "DISABLED");
    }
    public void ToggleVsyncCount(Text vsyncText)
    {
        if (QualitySettings.vSyncCount == 4) QualitySettings.vSyncCount = 0;
        else QualitySettings.vSyncCount++;
        SetVSyncCount(QualitySettings.vSyncCount);
        InitVsyncStateUI(vsyncText);
    }
    public void InitVsyncStateUI(Text vsyncText)
    {
        if (!PlayerPrefs.HasKey("vsync") || PlayerPrefs.GetInt("vsync") == 0)
        {
            vsyncText.text = "VSYNC: OFF";
        }
        else
        {
            vsyncText.text = "VSYNC:\n" + PlayerPrefs.GetInt("vsync") + " VBLANK";
        }
    }
    public void ResetHighscore()
    {
        PlayerPrefs.DeleteKey("highscore");
    }
    public void ChangeParallax(Text parallaxText)
    {
        if (!PlayerPrefs.HasKey("parallax"))
        {
            SetParallax(1.1f);
            parallaxText.text = "110%";
        }
        else if (Mathf.Approximately(PlayerPrefs.GetFloat("parallax"), 2f))
        {
            SetParallax(0f);
            parallaxText.text = "0%";
        }
        else
        {
            SetParallax(PlayerPrefs.GetFloat("parallax") + 0.1f);
            if (PlayerPrefs.HasKey("parallax")) GlobalVar.ParralaxSpeed = PlayerPrefs.GetFloat("parallax");
            InitParalaxStateUI(parallaxText);
        }   
    }
    public void InitParalaxStateUI(Text parallaxText)
    {
        if (!PlayerPrefs.HasKey("parallax"))
        {
            parallaxText.text = "100%";
        }
        else
        {
            parallaxText.text = Mathf.RoundToInt(PlayerPrefs.GetFloat("parallax") * 100) + "%";
        }
    }
    #endregion
    
    #region Functional Actions
    public static void SetAchivement(int value)
    {
        PlayerPrefs.SetInt("achivement", value);
        PlayerPrefs.Save();
    }
    public static void SetShaderState(bool value)
    {
        PlayerPrefs.SetInt("ShaderState", System.Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public static void SetHighScore(float value)
    {
        PlayerPrefs.SetFloat("highscore", value);
        PlayerPrefs.Save();
    }
    public static void SetVSyncCount(int value)
    {
        PlayerPrefs.SetInt("vsync", value);
        PlayerPrefs.Save();
    }
    public static void SetParallax(float value)
    {
        PlayerPrefs.SetFloat("parallax", value);
        PlayerPrefs.Save();
    }
    public static void SetHighSeed(int value)
    {
        PlayerPrefs.SetInt("highseed", value);
        PlayerPrefs.Save();
    }
    public static void SetHighMode(string mode, float value)
    {
        PlayerPrefs.SetString("highmode", mode);
        PlayerPrefs.SetFloat("highvalue", value);
        PlayerPrefs.Save();
    }

    public static int LoadAchivement()
    {
        if (PlayerPrefs.HasKey("achivement"))
        {
            return PlayerPrefs.GetInt("achivement");
        }
        else return 0;
    }
    public static bool LoadShaderState()
    {
        if (PlayerPrefs.HasKey("ShaderState"))
        {
            return System.Convert.ToBoolean(PlayerPrefs.GetInt("ShaderState"));
        }
        else return true;
    }
    public static float LoadHighScore()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            return PlayerPrefs.GetFloat("highscore");
        }
        else return -1f;
    }
    public static int LoadVSyncCount()
    {
        if (PlayerPrefs.HasKey("vsync"))
        {
            return PlayerPrefs.GetInt("vsync");
        }
        else return 0;
    }
    public static float LoadParallax()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            return PlayerPrefs.GetFloat("highscore");
        }
        else return -1f;
    }
    public static int LoadHighSeed()
    {
        if (PlayerPrefs.HasKey("highseed"))
        {
            return PlayerPrefs.GetInt("highseed");
        }
        else return -1;
    }
    public static KeyValuePair<string, float> LoadHighMode()
    {
        string mode = "Unknown";

        if (PlayerPrefs.HasKey("highmode"))
        {
            mode = PlayerPrefs.GetString("highmode");
        }

        float value = -1f;

        if (PlayerPrefs.HasKey("highvalue"))
        {
            value = PlayerPrefs.GetFloat("highvalue");
        }

        return new KeyValuePair<string, float>(mode, value);
    }
    #endregion

    /// <summary>
    /// Deal damage to an EntityInfo object with the specified tag, else does nothing.
    /// </summary>
    public static void DealDamage(GameObject obj, float amount, List<string> includedTag)
    {
        if (includedTag.Contains(obj.tag))
        {
            obj.GetComponent<EntityInfo>().OnDamage(amount);
        }
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadYourAsyncScene(sceneName));
    }
    public void ChangeVsyncCount(int amount)
    {
        QualitySettings.vSyncCount = amount;
    }
    public void ChangeFrameRate(int amount)
    {
        Application.targetFrameRate = amount;
    }

    public void ChangeQuality(int amount)
    {
        QualitySettings.SetQualityLevel(amount);
    }

    public void LoadSeed(InputField input)
    {
        if (!string.IsNullOrWhiteSpace(input.text))
        {
            try
            {
                GlobalVar.GameSeed = System.Convert.ToInt32(input.text);
            }
            catch (System.Exception)
            {
                GlobalVar.GameSeed = UnityEngine.Random.Range(0, int.MaxValue);
            }          
        }
        else
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] rand = new byte[4];
            rng.GetBytes(rand);
            GlobalVar.GameSeed = System.BitConverter.ToInt32(rand, 0);
        }
    }

    public void SetGameplayMode(Text seedText)
    {
        PlayerPrefs.SetString("Verb", seedText.text);
        PlayerPrefs.Save();
    }

    public void SetDetailedScore()
    {
        string extraInfo = "";

        if (Application.targetFrameRate == -1)
        {
            extraInfo = " - AVG. " + System.Math.Round(Time.frameCount / GlobalVar.GameTime, 2) + " FPS";
        }
        else extraInfo = " - SR. " + System.Math.Round(slowFramesTime / slowframeCount / Application.targetFrameRate * 100f, 2) + "%";

        seedText.text += extraInfo;
    }

    public void SetDetailedHigh()
    {
        highscoreText.text = "HIGH: " + System.TimeSpan.FromSeconds(LoadHighScore()).ToString().Replace("0000", "") + ", " + "SEED: " + LoadHighSeed() + ", " + LoadHighMode().Key + " " + System.Math.Round(LoadHighMode().Value, 2);

        if (LoadHighMode().Key == "SR.")
        {
            highscoreText.text += "%";
        }
        else if (LoadHighMode().Key == "AVG.")
        {
            highscoreText.text += " FPS";
        }
    }

    public void PauseGame(bool isPaused)
    {
        if (isPaused) currentTimeScale = Time.timeScale;

        Time.timeScale = (isPaused) ? 0.0f : currentTimeScale;

        player.GetComponent<PlayerMovement>().enabled = !isPaused;

        for (int i = 0; i < gunList.Length; i++)
        {
            gunList[i].enabled = !isPaused;
        }

        globalCanvas.GetComponent<GraphicRaycaster>().enabled = isPaused;    

        pausePanel.SetActive(isPaused);

        timeText.text = System.TimeSpan.FromSeconds(GlobalVar.GameTime).ToString().Replace("0000", "");
    }

    public void FinalizeGame()
    {
        if (!gameOverPanel.activeSelf)
        {
            globalCanvas.GetComponent<GraphicRaycaster>().enabled = true;
            StopAllCoroutines();
            Time.timeScale = 1f;
            GameOver();
            playerDeathSound.Play();
        } 
    }

    private void GameOver()
    {
        timeText.text = System.TimeSpan.FromSeconds(GlobalVar.GameTime).ToString().Replace("0000", "");

        gameOverPanel.SetActive(true);

        SetDetailedScore();

        if (sessionVerb == DEBUG_GUID || sessionVerb == BOSSRUSH_GUID)
        {
            switch (sessionVerb)
            {
                case BOSSRUSH_GUID:
                {
                    highscoreText.text = GlobalVar.GameBossDefeated + " BOSS(ES) DEFEATED - HIGHSCORE NOT SAVED";
                    break;
                }
                default:
                {
                    highscoreText.text = "EXTRA MODE - HIGHSCORE NOT SAVED";
                    break;
                }
            }
            if (vignetteManager.enabled) StartCoroutine(VignetteFadeout(5.0f));
            return;
        }
        else
        {
            GlobalProc.SetAchivement(GlobalVar.GameBossDefeated);
        }

        if (LoadHighScore() < GlobalVar.GameTime)
        {
            SetHighScore(GlobalVar.GameTime);

            SetHighSeed(GlobalVar.GameSeed);

            if (Application.targetFrameRate == -1)
            {
                SetHighMode("AVG.", (float)System.Math.Round(Time.frameCount / GlobalVar.GameTime, 2));
            }
            else SetHighMode("SR.", (float)System.Math.Round(slowFramesTime / slowframeCount / Application.targetFrameRate * 100f, 2));

            highscoreText.text = "NEW HIGH SCORE!";
        }
        else highscoreText.text = "Well done! - " + System.TimeSpan.FromSeconds(LoadHighScore() - GlobalVar.GameTime).ToString().Replace("0000", "") + " secs behind";
    
        if (vignetteManager.enabled) StartCoroutine(VignetteFadeout(5.0f));
    }

    private bool IsInRange(float target, float start, float limit)
    {
        return (target >= start) && (target < limit);
    }

    private void Awake() {
        self = this;
    }

    private void Start()
    {
        sessionVerb = PlayerPrefs.GetString("Verb");
        switch (sessionVerb)
        {
            case DEBUG_GUID:
            {
                player.entityHealth = 9999999f;
                this.GetComponent<GameController>().difficultySpeed *= 3f;
                debugText.gameObject.SetActive(true);
                break;
            }
            case BOSSRUSH_GUID:
            {
                player.entityHealth = 250f;
                this.GetComponent<GameController>().difficultySpeed = 0.5f;
                break;
            }
            case LOWLUXBACKGROUND_GUID:
            {
                backgroundRenderer.color = new Color(1f, 0.5f, 0f, 1f);
                break;
            }
            default:
            {
                break;
            }
        }
        if (PlayerPrefs.HasKey("parallax")) GlobalVar.ParralaxSpeed = PlayerPrefs.GetFloat("parallax");

        player = GlobalVar.self.player;
        gameTimeDelayTimer = gameTimeDelay;
        gameMultiplierText.text = System.Math.Round(GlobalVar.GameDifficulty, 2).ToString();
        GlobalVar.self.player.OnDamage(0); // Set the player health to GUI
        UnityEngine.Random.InitState(GlobalVar.GameSeed); // Set the random seed
        seedText.text = "SEED: " + GlobalVar.GameSeed;
        ResetTimescale();
        vignetteManager.enabled = LoadShaderState();
    }
    private void Update()
    {
        if (player == null) return;

        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame(!pausePanel.gameObject.activeSelf);
        } 

        gameTimeDelayTimer -= Time.deltaTime;

        if (gameTimeDelayTimer <= 0f)
        {
            timeText.text = System.TimeSpan.FromSeconds(GlobalVar.GameTime).ToString("hh':'mm':'ss'.'fff");
            gameMultiplierText.text = System.Math.Round(GlobalVar.GameDifficulty, 2).ToString();
            gameTimeDelayTimer = gameTimeDelay;
        }

        if (GlobalVar.GameTime - gameTimeStamp >= playerBonusThreshold)
        {
            gameTimeStamp = GlobalVar.GameTime;
            if (GlobalVar.GameDifficulty < 1.3f) return;
            if (IsInRange(GlobalVar.GameDifficulty, 1.3f, 1.7f)) // 0.4
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus);
            }
            else if (IsInRange(GlobalVar.GameDifficulty, 1.7f, 2.3f)) // 0.6
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 2);
            }
			else if (IsInRange(GlobalVar.GameDifficulty, 2.3f, 3f)) // 0.7
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 3);
            }
            else if (IsInRange(GlobalVar.GameDifficulty, 3f, 4.6f)) // 1.6
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 4);
            } 
            else if (IsInRange(GlobalVar.GameDifficulty, 4.6f, 5.5f)) // 0.9
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 5);
            }   
			else if (IsInRange(GlobalVar.GameDifficulty, 5.5f, 6.3f)) // 0.8
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 6);
            }             
            else if (IsInRange(GlobalVar.GameDifficulty, 6.3f, 7f)) // 0.7
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * 7); 
            }
            else if (GlobalVar.GameDifficulty >= 7f)
            {
                GlobalVar.self.player.OnDamage(-playerHealthBonus * (7 + Mathf.Floor(GlobalVar.GameBossDefeated / 5)));
            }  
            player.ChangeHealthColor(new Color(0.4f, 0.6902f, 0.1961f), 0.5f); // GreenRYB
        }

        GlobalVar.GameTime += Time.deltaTime;

        if (Application.targetFrameRate == -1) return;

        float frame = 1 / Time.deltaTime;

        if (frame < Application.targetFrameRate)
        {
            slowFramesTime += frame;
            slowframeCount++;
        }
    }

    IEnumerator VignetteFadeout(float time)
    {
        // Changing shader values required a frame to be drawn
        yield return new WaitForEndOfFrame();
        float falloffDistance = Mathf.Clamp(10f - vignetteManager.VignetteFalloff, 0f, 10f);
        float innerDistance = Mathf.Clamp(1f - vignetteManager.VignetteInnerSaturation, 0f, 1f);
        float outerDistance = Mathf.Clamp(1f - vignetteManager.VignetteOuterSaturation, 0f, 1f);
        while (vignetteManager.VignetteFalloff < 10f || vignetteManager.VignetteInnerSaturation < 1f || vignetteManager.VignetteOuterSaturation < 1f)
        {
            vignetteManager.VignetteFalloff =
                Mathf.Clamp(vignetteManager.VignetteFalloff + falloffDistance / time * Time.deltaTime, 0f, 10f);
            vignetteManager.VignetteInnerSaturation =
                Mathf.Clamp(vignetteManager.VignetteInnerSaturation + innerDistance / time * Time.deltaTime, 0f, 1f);
            vignetteManager.VignetteOuterSaturation = 
                Mathf.Clamp(vignetteManager.VignetteOuterSaturation + outerDistance / time * Time.deltaTime, 0f, 1f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {         
            yield return null;
        }

        Time.timeScale = 1.0f;
    }
}
