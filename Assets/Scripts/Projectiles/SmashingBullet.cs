using UnityEngine;

public class SmashingBullet : Bullet {
    
    [Header("Non-inheritence definitions")]
    public float firingInterval = 1f;
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingBurstSize = 1;
    public float radius = 1f;
    public float homingTime = 1f;    
    public GameObject firingBullet;
    public GameObject explosionObject;

    [SerializeField] private float explosionLayerThreshold = 0.5f;
    private float homingTimeTimer = 0f; 
    private float firingIntervalTimer = 0f;
    private bool isLaunching = false;

    private void AimAtPlayer()
    {
        if (playerHandler == null) return;
        Vector3 difference = playerHandler.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public override void OnDeath()
    {
        if (spriteRenderer.isVisible) 
        {
            Instantiate(explosionObject, transform.position, Quaternion.identity);
        }
        base.OnDeath();
    }

    private void FireBullet()
    {
        for (int i = 1; i <= firingBurstSize; i++)
        {
            float angle = (360f / firingBurstSize * i) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 fluctuatingVector =  new Vector3(x, y, 0f);
            GameObject obj = Instantiate(firingBullet, transform.position + fluctuatingVector, Quaternion.identity);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
        }     
    }

    protected override void OnStart()
    {
        base.OnStart();
        firingInterval = Mathf.Clamp(firingInterval / GlobalVar.GameDifficulty, 0f, float.MaxValue);
        firingIntervalTimer = firingInterval;
        homingTimeTimer = homingTime;
        speed *= (GlobalVar.GameDifficulty / 1.5f >= 1f) ? GlobalVar.GameDifficulty / 1.5f : 1f;
        AimAtPlayer();
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        if (isLaunching)
        {
            bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position + transform.right * speed * Time.fixedDeltaTime);

            if (firingIntervalTimer <= 0f)
            {
                FireBullet();
                firingIntervalTimer = firingInterval;
            }
            else firingIntervalTimer -= Time.fixedDeltaTime;
        }
        else
        {
            homingTimeTimer -= Time.fixedDeltaTime;
            AimAtPlayer();
            if (homingTimeTimer <= 0f) isLaunching = true;
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
}