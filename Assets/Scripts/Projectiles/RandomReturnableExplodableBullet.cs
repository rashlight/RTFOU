using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomReturnableExplodableBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingBurstSize = 1;
    public GameObject firingBullet;
    public bool isDirectionReversed = false;

    public float returnTime;
    public float returnAmount; 
    private float returnTimeTimer = 0f;
    private bool isHomingMode = false;
    private Vector3 dir;

    protected override void OnStart()
    {
        base.OnStart();
        returnTimeTimer = returnTime;
        dir = (isDirectionReversed) ? transform.right : -transform.right;
    }

    public override void OnDeath()
    {
        for (int i = 1; i <= firingBurstSize; i++)
        {
            GameObject obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            obj.transform.rotation = Quaternion.Euler(0f, 0f, 360f / firingBurstSize * i);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
        }
        base.OnDeath();
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

