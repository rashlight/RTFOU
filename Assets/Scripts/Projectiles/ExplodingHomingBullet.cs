using UnityEngine;

public class ExplodingHomingBullet : Bullet {

    [Header("Non-inheritence definitions")]
    public int firingAmount = 1;
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public float homingTime = 1f;
    public float homingMultiplier = 0.1f;
    public HomingBullet firingBullet;
    public GameObject explosionObject;
    
    [SerializeField] private float explosionLayerThreshold = 0.5f;

    protected override void OnStart()
    {
        base.OnStart();
        speed *= (GlobalVar.GameDifficulty / 1.5f >= 1f) ? GlobalVar.GameDifficulty / 1.5f : 1f;
    }

    void FireBullet()
    {
        for (int i = 1; i <= firingAmount; i++)
        {
            HomingBullet obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            obj.damage = firingDamage;
            obj.speed = firingSpeed;
            obj.homingTime = (homingTime + homingMultiplier * i);
            obj.ResetHomingTimer();
        }  
    }

    public override void OnDeath()
    {
        FireBullet();
        if (spriteRenderer.isVisible) 
        {
            Instantiate(explosionObject, transform.position, Quaternion.identity);
        }
        base.OnDeath();
    }

    private void FixedUpdate() {
        bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - transform.right * speed * Time.fixedDeltaTime);
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