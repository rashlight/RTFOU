using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public bool isDirectionReversed = false;

    protected override void OnStart()
    {
        speed *= (GlobalVar.GameDifficulty / 1.5f >= 1f) ? GlobalVar.GameDifficulty / 1.5f : 1f;
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        Vector3 dir = (isDirectionReversed) ? transform.right : -transform.right;
        bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - dir * speed * Time.fixedDeltaTime);
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

