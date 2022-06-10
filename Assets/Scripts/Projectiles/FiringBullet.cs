using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringBullet : Bullet
{
    [Header("Non-inheritence definitions")]
    public float firingInterval = 1f;
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingTime = 1;
    public bool isAngleRandom = false;
    public bool isAffectedByDifficulty = false;
    public Vector3 staticFiringAngle;
    public GameObject firingBullet;
    public GameObject explosionObject;

    [SerializeField] private float explosionLayerThreshold = 0.5f;
    private float firingIntervalTimer = 0f;

    private void FireBullet()
    {
        for (int i = 0; i < firingTime; i++)
        {
            GameObject obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
            if (isAngleRandom)
            {
                obj.transform.Rotate(new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            }
            else obj.transform.rotation = Quaternion.Euler(staticFiringAngle);
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
        if (isAffectedByDifficulty) 
        {
            damage = Mathf.Round(damage * GlobalVar.GameDifficulty / 1.2f);
            firingInterval /= GlobalVar.GameDifficulty * 1.3f;
        }
        firingIntervalTimer = firingInterval;
    }

    // Called once or more per frame
    void FixedUpdate()
    {
        bulletRigidbody.MovePosition((Vector3)bulletRigidbody.position - new Vector3(
            speed * Time.deltaTime,
            0f,
            0f
        ));

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
