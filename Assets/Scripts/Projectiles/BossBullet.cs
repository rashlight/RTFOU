using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Defines a boss bullet (disabling GameController until killed)
/// </summary>
public class BossBullet : Bullet {
    public GameObject[] bossSummoners;
    public EntityPattern introPattern;
    public EntityPattern lastWordPattern;
    public EntityPattern[] bossPatterns;
    public GameObject explosionObject;
    public float hurtDelay;
    public float stopTime;
    [HideInInspector] public int selectedGunIndex;
    [HideInInspector] public bool isAttacking = false;
    public Color bossColor;
    protected float summonDelayTimer;
    protected bool isLastWordEnabled = false;
    [SerializeField] private float extraJumpMultiplier = 1.6f;
    private float defaultPlatformHorizontal = 0f; // Temporary stored to change platform stance
    private float defaultPlatformVertical = 0f; // Temporary stored to change platform stance
    private float defaultJumpForce = 0f; // Temporary stored to change player's property
    private float defaultLowJumpMultiplier = 0f; // Temporary stored to change player's property
    private float defaultStopTime = 0f;
    private PlayerMovement playerMovement;
    private GameController controller;
    [SerializeField] private Color hurtColor = Color.white;
    private Coroutine hurtCoroutine;

    public void ApplyExtraJump()
    {
        defaultJumpForce = playerMovement.jumpForce;
        playerMovement.jumpForce *= extraJumpMultiplier;
        defaultLowJumpMultiplier = playerMovement.lowJumpMultiplier;
        playerMovement.lowJumpMultiplier *= extraJumpMultiplier;
    }

    public void SetBossText()
    {
        System.TimeSpan stopTimeSpan = System.TimeSpan.FromSeconds(stopTime);
        GlobalVar.self.bossText.text = Mathf.Round(entityHealth) + " \\ " + stopTimeSpan.ToString("mm':'ss'.'f");
    }

    private void AimAtPlayer()
    {
        if (playerHandler == null) return;
        Vector3 difference = playerHandler.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg - 180f;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public virtual void SummonIntro()
    {
        summonDelayTimer = introPattern.sequenceDelay;
        introPattern.StartSequence();
    }

    public virtual void SummonRandomPattern()
    {
        EntityPattern ep = bossPatterns[Random.Range(0, bossPatterns.Length)];
        summonDelayTimer = ep.sequenceDelay * (1f / GlobalVar.GameDifficulty);
        ep.StartSequence();
    }

    public virtual void SummmonLastWord()
    {
        summonDelayTimer = lastWordPattern.sequenceDelay;
        lastWordPattern.StartSequence();
    }

    public virtual void OnFixedUpdate()
    {

    }

    public override void OnDamage(float amount)
    {
        SetBossText();
        if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);
        hurtCoroutine = StartCoroutine(HurtFlash());
        base.OnDamage(amount);
    }

    public override void OnDeath()
    {
        // Reset extra stuff to normal
        if (playerHandler != null && playerHandler.entityHealth > 0f)
        {
            // Reset platform stances
            GlobalVar.PlatformFluctuationHorizontal = defaultPlatformHorizontal;
            GlobalVar.PlatformFluctuationVertical = defaultPlatformVertical;
            playerMovement.jumpForce = defaultJumpForce;
            playerMovement.lowJumpMultiplier  = defaultLowJumpMultiplier;
            // Assign boss defeat
            GlobalVar.GameBossDefeated++;
            // Disable and reset gun
            playerHandler.playerWeapons[selectedGunIndex].gameObject.SetActive(false);
            playerHandler.playerWeapons[selectedGunIndex].GetComponent<Gun>().ResetGun();
            // Heal player for x2 health or to (250 * difficulty)hp, multiplied by time.
            float healthEarned = (playerHandler.entityHealth * 2f < 200f * (GlobalVar.GameDifficulty - 1f)) ? playerHandler.entityHealth * 2f : 200f * (GlobalVar.GameDifficulty - 1f);
            float degradeRewardTime = Mathf.Round(defaultStopTime / 4);
            if (((stopTime < 0f) ? 0 : stopTime) < degradeRewardTime) healthEarned *= stopTime / degradeRewardTime;
            // Flooring, yes.
            healthEarned = Mathf.Floor(healthEarned);
            playerHandler.OnHeal(healthEarned);
            playerHandler.entityHealth = Mathf.Round(playerHandler.entityHealth);
            controller.enabled = true;
            // Summon explosion
            Instantiate(
                explosionObject, 
                new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), 
                Quaternion.identity
            );
        } 
        // Disable UI
        GlobalVar.self.bossImage.gameObject.SetActive(false);
        GlobalVar.self.bossText.gameObject.SetActive(false);
        base.OnDeath();
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (GlobalVar.self.player == null) return;
        controller = UnityEngine.EventSystems.EventSystem.current.GetComponent<GameController>();
        playerMovement = GlobalVar.self.player.GetComponent<PlayerMovement>();
        // Does last word is enabled?
        isLastWordEnabled = GlobalVar.GameBossDefeated >= controller.bossBullets.Length;
        // Change platform stances
        defaultPlatformHorizontal = GlobalVar.PlatformFluctuationHorizontal;
        defaultPlatformVertical = GlobalVar.PlatformFluctuationVertical;
        GlobalVar.PlatformFluctuationHorizontal *= 0.6f;
        GlobalVar.PlatformFluctuationVertical = 0f;
        // Buffs and configs
        entityHealth *= GlobalVar.GameDifficulty - ((GlobalVar.GameDifficulty <= 2f) ? 1f : 0.5f);
        if (GlobalVar.GameDifficulty >= 5f) entityHealth *= 1.5f;
        stopTime += stopTime * (GlobalVar.GameDifficulty - 2f) * 0.2f;
        despawnDelay = float.PositiveInfinity;
        damage = float.PositiveInfinity; // touching boss = death
        selectedGunIndex = Random.Range(0, GlobalVar.self.player.playerWeapons.Length);
        defaultStopTime = stopTime;
        // Enable boss image and text
        GlobalVar.self.bossImage.gameObject.SetActive(true);
        GlobalVar.self.bossText.gameObject.SetActive(true);
    }

    private void FixedUpdate() {
        OnFixedUpdate();
        if (stopTime <= 0f)
        {
            if (!isAttacking || GlobalVar.self.player == null) return;
            // Aim and kill player
            AimAtPlayer();
            bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - transform.right * speed * Time.fixedDeltaTime);
            GlobalVar.self.bossText.text = "! CAUTION !";
        }
        else if (isAttacking)
        {
            stopTime -= Time.fixedDeltaTime;
            SetBossText();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collideObject = collision.gameObject;

        GlobalProc.DealDamage(collideObject, damage, includedDamageTags);
        if (CheckLayerMask(collideObject.layer, healthCheckLayerMask))
        {
            OnDamage(collideObject.GetComponent<Bullet>().damage);
        }
        else OnDamage(entityHealth);
    }

    IEnumerator HurtFlash()
    {
        spriteRenderer.color = hurtColor;
        yield return new WaitForSeconds(hurtDelay);
        spriteRenderer.color = Color.white;
    }
}