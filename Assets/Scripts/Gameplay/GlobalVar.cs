using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlobalVar : MonoBehaviour
{
    public static int GameBossDefeated = 4;
    public static float GameTime = 0f;
    public static int GameSeed = 0;
    public static float GameDifficulty = 0f;

    public static float PlatformSpeed = 0f;
    public static float PlatformFluctuationHorizontal = 0f;
    public static float PlatformFluctuationVertical = 0f;

    public static float ParralaxSpeed = 1f;

    /// <summary>
    /// Reference to GlobalVar (this). Use with extreme caution.
    /// </summary>
    public static GlobalVar self;
    
    [Header("Referenced Objects Area")]
    public Player player;
    public GameObject[] platformSlices;
    public Image bossImage;
    public Text bossText;
    public Color defaultTextColor;

    private void Awake() {
        self = this;
        GameTime = 0f;
        GameDifficulty = 1f;
        PlatformSpeed = 6.5f;
        PlatformFluctuationHorizontal = 8.5f;
        PlatformFluctuationVertical = 2.5f;
        if (PlayerPrefs.HasKey("parallax")) GlobalVar.ParralaxSpeed = PlayerPrefs.GetFloat("parallax");
    }
}