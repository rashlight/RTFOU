using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomReturnableHomingBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public bool isDirectionReversed = false;
    public int chance = 20;
    public int chanceOverideLimit = 50;
    public float returnTime;
    private float returnTimeTimer = 0f;
    private int chanceTime = 0;
    private bool isHomingMode = false;
    private Vector3 dir;

    private void AimAtPlayer()
    {
        if (playerHandler == null) return;
        Vector3 difference = playerHandler.transform.position - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    protected override void OnStart()
    {
        base.OnStart();
        returnTimeTimer = returnTime;
        dir = (isDirectionReversed) ? transform.right : -transform.right;
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        if (returnTimeTimer <= 0f && !isHomingMode)
        {
            if (Random.Range(0, 101) <= chance || chanceTime == chanceOverideLimit)
            {
                AimAtPlayer();
                isHomingMode = true;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 359f));
                chanceTime++;
            } 
            returnTimeTimer = returnTime; 
        } 
        dir = (isDirectionReversed) ? transform.right : -transform.right;
        bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - dir * speed * Time.fixedDeltaTime);
        returnTimeTimer -= Time.fixedDeltaTime;
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

