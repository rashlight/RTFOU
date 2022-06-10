using UnityEngine;

public class RoundBurstBullet : Bullet {
    
    [Header("Non-inheritence definitions")]
    public float firingInterval = 1f;
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingBurstSize = 1;
    public int returnChance = 25;

    public GameObject firingBullet;
    public GameObject explosionObject;

    [SerializeField] private float explosionLayerThreshold = 0.5f;
    private float firingIntervalTimer = 0f;
    private bool isReturning = false;

    private void FireBullet()
    {      
        if (Random.Range(1f, 100f) <= returnChance)
        {
            isReturning = true;
            firingBurstSize *= 2;
            speed *= 5f;
        } 

        for (int i = 1; i <= firingBurstSize; i++)
        {
            GameObject obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 360f / firingBurstSize * i);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
        }     
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
        firingInterval = Mathf.Clamp(firingInterval / GlobalVar.GameDifficulty / 1.1f, 0f, float.MaxValue);
        firingIntervalTimer = firingInterval;
        speed *= (GlobalVar.GameDifficulty / 1.5f >= 1f) ? GlobalVar.GameDifficulty / 1.5f : 1f;
        firingBurstSize *= Mathf.FloorToInt(Mathf.Clamp(GlobalVar.GameDifficulty / 2f, 1f, float.MaxValue));
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        if (isReturning)
        {
            bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position + new Vector3(
                speed * Time.deltaTime,
                0f,
                0f
            ));
        }
        else
        {
            bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - new Vector3(
                speed * Time.deltaTime,
                0f,
                0f
            ));
        }
        

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