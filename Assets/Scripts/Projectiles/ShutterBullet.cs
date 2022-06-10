using UnityEngine;

public class ShutterBullet : Bullet {
    [Header("Non-inheritence definitions")]
    public float firingInterval = 1f;
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingBurstSize = 1;
    public float distanceThreshold = 3f;
    public GameObject firingBullet;
    public GameObject explosionObject;

    [SerializeField] private float explosionLayerThreshold = 0.5f;
    private float firingIntervalTimer = 0f;

    private void AimAtPlayer()
    {
        if (playerHandler == null) return;
        Vector3 difference = playerHandler.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        if (Vector2.Distance(playerHandler.transform.position, transform.position) <= distanceThreshold) 
        {
            speed *= 2f;
            distanceThreshold = -1f;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    private void FireBullet()
    {
        for (int i = 1; i <= firingBurstSize; i++)
        {
            GameObject obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 360f / firingBurstSize * i);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
        }   

        AimAtPlayer();
    }

    public override void OnDeath()
    {
        if (spriteRenderer.isVisible) 
        {
            Instantiate(explosionObject, transform.position, Quaternion.identity);
        }
        base.OnDeath();
    }

    protected override void OnStart()
    {
        base.OnStart();
        firingIntervalTimer = firingInterval;
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position + transform.right * speed * Time.fixedDeltaTime);

        if (firingIntervalTimer <= 0f)
        {
            FireBullet();
            firingIntervalTimer = firingInterval;
        }
        else firingIntervalTimer -= Time.fixedDeltaTime;
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