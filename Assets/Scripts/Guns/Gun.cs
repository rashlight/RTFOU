using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Fires bullets, according to player input(s).
/// </summary>
public abstract class Gun : MonoBehaviour {
    
    public float damage;
    public float fireRate;
    public float spread;
    public int magazineSize;
    public float reloadRate;
    public int bulletAmount;
    public float fireRateTimer;
    public float reloadRateTimer;
    public bool isReloadMode = false;
    public bool isPulledTrigger = false;
    [SerializeField] protected Bullet summonBullet;
    [SerializeField] private float muzzleFlashTime;
    [SerializeField] private GameObject firingPivot;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Image gunStatusImage;
    [SerializeField] private Text gunStatusText;
    [SerializeField] private Sprite displaySprite;
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private AudioSource ejectMagazineSound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private AudioSource attachMagazineSound;
    private Color defaultGunTextColor;
    private Quaternion gunDefaultRotation;
    private GunProperties defaultGun;
    private Coroutine muzzleFlashing;

    /// <summary>
    /// Make the gun resets to default setting.
    /// </summary>
    public void ResetGun()
    {
        defaultGun.SetProperties(this);  
        transform.rotation = gunDefaultRotation;
    }

    public virtual void OnStart()
    {
        
    }

    public virtual void OnFire()
    {
        if (muzzleFlashing != null) StopCoroutine(muzzleFlashing);
        muzzleFlashing = StartCoroutine(MuzzleFlashing(muzzleFlashTime));
    }

    public virtual void OnOutOfAmmo()
    {
        gunStatusText.color = new Color(0.6118f, 0.5333f, 0.3333f); //DesertYellow
    }

    public virtual void OnReload()
    {

    }

    public virtual void OnAmmoRechargeFinished()
    {
        gunStatusText.color = defaultGunTextColor;
    }

    protected virtual void OnDisable() {
        if (gunStatusImage == null || gunStatusText == null) return;
        StopAllCoroutines();
        muzzleFlash.SetActive(false);
        gunStatusImage.gameObject.SetActive(false);
        gunStatusText.gameObject.SetActive(false);
        gunStatusText.color = defaultGunTextColor;
    }

    protected virtual void OnEnable() {
        if (gunStatusImage == null || gunStatusText == null) return;
        gunStatusImage.sprite = displaySprite;
        gunStatusImage.gameObject.SetActive(true);
        gunStatusText.gameObject.SetActive(true);
        UpdateGunStatus();
    }

    protected void UpdateGunStatus()
    {
        gunStatusText.text = bulletAmount + " / " + magazineSize;
    }

    protected void FireBullet()
    {
        Vector3 turnAngle = transform.rotation.eulerAngles;
        summonBullet.damage = this.damage;

        float orientation = gunDefaultRotation.eulerAngles.z + Random.Range(-spread, spread) % 360;

        Quaternion spreadRotation = Quaternion.Euler(
            0f, 
            0f, 
            orientation
        );

        transform.rotation = spreadRotation;

        firingPivot.transform.rotation = spreadRotation;

        Instantiate(summonBullet, firingPivot.transform.position, firingPivot.transform.rotation);

        /* Unused - angle clamping
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            Mathf.Clamp(orientation, gunThresholdDown, gunThresholdUp)
        );
        */

        firingPivot.transform.rotation = transform.rotation;
            
        bulletAmount--;

        OnFire();

        if (bulletAmount != 0) 
        {
            fireSound.Play();
        }
        else 
        {
            isReloadMode = true;
            OnOutOfAmmo();
            ejectMagazineSound.Play();
        }
    }

    private void Start() {
        fireRateTimer = fireRate;
        reloadRateTimer = reloadRate;
        bulletAmount = magazineSize;
        gunDefaultRotation = transform.rotation;
        defaultGun = new GunProperties(this);
        gunStatusImage.sprite = summonBullet.spriteRenderer.sprite;
        OnStart();
        defaultGunTextColor = gunStatusText.color;
        UpdateGunStatus();
    }

    private void Update() {
        // Changes from last reload -> fire mode
        if (isReloadMode && isPulledTrigger && Input.GetButtonUp("Jump"))
        {
            isPulledTrigger = false;
            isReloadMode = false;
        } 

        // Changes from fire mode -> reload mode
        if (isReloadMode && !isPulledTrigger && reloadRateTimer <= 0f && Input.GetButtonDown("Jump"))
        {
            reloadRateTimer = reloadRate;
            bulletAmount = Mathf.Clamp(bulletAmount + Mathf.FloorToInt(Random.Range(magazineSize / 10, magazineSize / 5)) , 0, magazineSize);
            if (bulletAmount == magazineSize) 
            {
                isPulledTrigger = true;
                OnAmmoRechargeFinished();
                UpdateGunStatus();
                attachMagazineSound.Play();
            }
            else 
            {
                OnReload();
                UpdateGunStatus();
                reloadSound.Play();
            }
        }        

        // Fire mode
        if (!isReloadMode && fireRateTimer <= 0f && Input.GetButton("Jump"))
        {
            FireBullet();
            fireRateTimer = fireRate;
        }
    }

    private void FixedUpdate() {
        fireRateTimer -= Time.fixedDeltaTime;
        reloadRateTimer -= Time.fixedDeltaTime;
    }

    IEnumerator MuzzleFlashing(float time)
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(time);
        muzzleFlash.SetActive(false);
        yield return null;
    }
}