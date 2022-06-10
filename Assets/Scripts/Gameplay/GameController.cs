using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour
{
    [Tooltip("Taken by fixedDeltaTime")]
    public float difficultySpeed;
    public float summonDelay = 2.5f;
    public Transform bossSummonPivot;
    public BulletSummoner[] summonPivots;
    public EntityPattern[] entityPatterns;
    public GameObject[] bossBullets;
    [SerializeField] private float difficulty = 1f;
    [SerializeField] private float platformDifficultyLimit = 3f;
    private float difficultySpeedTimer;
    private float summonDelayTimer = 0f;
    private float defaultPlatformSpeed = 0f;

    private static bool CheckBossThreshold(float number, float higherOrEqualThan = float.MinValue, float lowerThan = float.MaxValue)
    {
        if (number >= higherOrEqualThan && number < lowerThan)
        {
            return true;
        }
        else return false;
    }

    private void Start()
    {
        summonDelayTimer = summonDelay;
        difficultySpeedTimer = difficulty;
        GlobalVar.GameBossDefeated = 0;
        defaultPlatformSpeed = GlobalVar.PlatformSpeed;
    }

    private void FixedUpdate()
    {
        // Increases game difficulty
        difficultySpeedTimer = Mathf.Clamp(difficultySpeedTimer + difficultySpeed * Time.fixedDeltaTime, 1f, GlobalVar.GameBossDefeated + 2f);    
        GlobalVar.GameDifficulty = difficultySpeedTimer;

        // Check if a boss can be spawned
        if (Mathf.Approximately(difficultySpeedTimer, GlobalVar.GameBossDefeated + 2f))
        {
            difficultySpeedTimer = Mathf.Clamp(difficultySpeedTimer, 1f, GlobalVar.GameBossDefeated + 2f);
            if (GlobalVar.GameBossDefeated + 1 <= bossBullets.Length) Instantiate(bossBullets[GlobalVar.GameBossDefeated], bossSummonPivot.position, Quaternion.identity);
            else Instantiate(bossBullets[Random.Range(0, bossBullets.Length)], bossSummonPivot.position, Quaternion.identity);
            this.enabled = false;
            return;
        }

        summonDelayTimer -= Time.fixedDeltaTime;
        if (summonDelayTimer <= 0f)
        {
            // Generates a random summon pattern and starts it
            EntityPattern sp = entityPatterns[Random.Range(0, entityPatterns.Length)];
            summonDelayTimer = sp.sequenceDelay * (1f / GlobalVar.GameDifficulty);
            StartCoroutine(sp.StartSequence());
        }

        // Affect all summoned platforms afterwards
        GlobalVar.PlatformSpeed = defaultPlatformSpeed * Mathf.Clamp(difficultySpeedTimer, 0f, platformDifficultyLimit);
    }
}
