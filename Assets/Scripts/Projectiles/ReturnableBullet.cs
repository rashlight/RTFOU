using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnableBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public bool isDirectionReversed = false;
    public float returnTime;
    public float returnAmount;
    public GameObject explosionObject;
    private float returnTimeTimer = 0f;
    private Vector3 dir;

    protected override void OnStart()
    {
        base.OnStart();
        returnTimeTimer = returnTime;
        dir = (isDirectionReversed) ? transform.right : -transform.right;
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        if (returnAmount > 0 && returnTimeTimer <= 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 359f));
            returnAmount--;
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

