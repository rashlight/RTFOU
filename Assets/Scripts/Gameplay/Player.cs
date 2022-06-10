using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class Player : EntityInfo {

    public float despawnDelayLimit = 0.5f;
    [Range(0f, 1f)]
    public float respawnSlowMultiplier;
    public Text playerHealthText;
    public Transform playerRespawn;
    [HideInInspector] public Color defaultHealthColor;
    [HideInInspector] public Coroutine healthColorCoroutine;
    public Gun[] playerWeapons;

    private float playerDefaultHealth;
    private float despawnDelay = 0.5f;
    private Rigidbody2D rb;
    private Renderer viewpointRenderer;
    private PlayerMovement playerMovement;
    [SerializeField] private Wilberforce.FinalVignette.FinalVignette vignetteManager;
    [SerializeField] private AudioSource warningSound;

    private void CheckDespawnDelay()
    {
        if (!viewpointRenderer.isVisible)
        {
            despawnDelay -= Time.deltaTime;
            if (despawnDelay <= 0f)
            {
                if (entityHealth > 100f)
                {
                    transform.position = playerRespawn.position;
                    rb.velocity = Vector2.down / 2f;
                    rb.position = playerRespawn.position;
                    despawnDelay = 0.5f;
                    OnDamage(100f);
                    StartCoroutine(playerMovement.SlowTimeAfterDeathSequence(respawnSlowMultiplier));
                }
                else OnDamage(entityHealth);
            }
        }
    }

    public void ChangeHealthColor(Color color, float time)
    {
        if (healthColorCoroutine != null) StopCoroutine(healthColorCoroutine);
        healthColorCoroutine = StartCoroutine(HealthColorFlash(color, time));
    }

    public override void OnDamage(float amount)
    {
        base.OnDamage(amount);
        playerHealthText.text = entityHealth.ToString();
        if (amount > 0f)
        {
            ChangeHealthColor(new Color(0.9215f, 0.3137f, 0.1882f), 0.5f); // TomatoRed
        }
        vignetteManager.VignetteFalloff = Mathf.Clamp(entityHealth / playerDefaultHealth * 9f + 1f, 1f, 10f);
        vignetteManager.VignetteInnerSaturation = Mathf.Clamp(entityHealth / playerDefaultHealth * 5f, 0f, 1f);
        vignetteManager.VignetteOuterSaturation = Mathf.Clamp(entityHealth / playerDefaultHealth, 0f, 1f);
        if (amount > 0f && entityHealth <= playerDefaultHealth * 0.2f && entityHealth > 0f) 
        {
            warningSound.Stop();
            warningSound.Play();
        }
    }

    public override void OnDeath()
    {
        GlobalProc.self.FinalizeGame();
        base.OnDeath();    
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        viewpointRenderer = GetComponent<Renderer>();
        playerMovement = GetComponent<PlayerMovement>();
        despawnDelay = despawnDelayLimit;
        playerDefaultHealth = entityHealth;
        defaultHealthColor = playerHealthText.color;
    }

    private void Update()
    {
        CheckDespawnDelay();
        if (entityHealth <= 0f) OnDeath();
    }

    public IEnumerator HealthColorFlash(Color color, float time)
    {
        playerHealthText.color = color;
        yield return new WaitForSeconds(time);
        playerHealthText.color = defaultHealthColor;
        yield return null;
    }
}
