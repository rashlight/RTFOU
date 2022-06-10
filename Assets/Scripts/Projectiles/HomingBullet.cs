using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public float homingTime = 1f;
    [Tooltip("Old aiming system")]
    public bool isAimOnce = false;

    private float homingTimeTimer = 0f; 
    private bool isFinishesAiming = false;
    
    public void ResetHomingTimer()
    {
        homingTimeTimer = homingTime;
    }

    protected override void OnStart()
    {
        base.OnStart();
        homingTimeTimer = homingTime;
        speed *= (GlobalVar.GameDifficulty / 1.5f >= 1f) ? GlobalVar.GameDifficulty / 1.5f : 1f;
        AimAtPlayer();
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        if (homingTimeTimer <= 0f)
        {
            bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position + transform.right * speed * Time.fixedDeltaTime);
        }
        else
        {
            homingTimeTimer -= Time.fixedDeltaTime;
            if (!isFinishesAiming) 
            {
                AimAtPlayer();
                if (isAimOnce) isFinishesAiming = true;
            }
        }
    }

    private void AimAtPlayer()
    {
        if (playerHandler == null) return;
        Vector3 difference = playerHandler.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
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
