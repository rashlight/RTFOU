using System.Collections.Generic;
using UnityEngine;
 
public class SplashBullet : Bullet {

    [Header("Non-inheritence definitions")]
    public bool isRandomLaunchAngle = true;
    public bool isGroundInteractable = false;
    public float startingForce = 250f;
    public Vector2 launchingVector = Vector2.up;

    [Tooltip("Fỉring Attributes")]
    public float firingDamage = 1f;
    public float firingSpeed = 5f;
    public int firingTime = 1;
    public bool isAngleRandom = false;
    public Vector3 staticFiringAngle;
    public GameObject firingBullet;
    public GameObject explosionObject;

    [SerializeField] private float explosionLayerThreshold = 0.5f;

    public override void OnDeath()
    {
        if (spriteRenderer.isVisible) 
        {
            Instantiate(explosionObject, transform.position, Quaternion.identity);
        }
        base.OnDeath();
    }

    protected void FireBullet()
    {
        for (int i = 0; i < firingTime; i++)
        {
            GameObject obj = Instantiate(firingBullet, transform.position, Quaternion.identity);
            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.damage = firingDamage;
            bullet.speed = firingSpeed;
            if (isAngleRandom)
            {
                obj.transform.Rotate(new Vector3(0f, 0f, UnityEngine.Random.Range(-30f, -150f)));
                obj.layer = LayerMask.NameToLayer("SpecialEnemy");
            }
            else obj.transform.rotation = Quaternion.Euler(staticFiringAngle);
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (isRandomLaunchAngle) 
        {
            if (launchingVector.x > 0f) launchingVector.x *= Random.Range(-2f, 2f);
            else launchingVector.x = Random.Range(-2f, 2f);
        }
        bulletRigidbody.AddForce(startingForce * launchingVector);
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        GameObject collideObject = collision.gameObject;

        if (isGroundInteractable && collideObject.layer == LayerMask.NameToLayer("Ground")) return;

        GlobalProc.DealDamage(collideObject, damage, includedDamageTags);
        if (CheckLayerMask(collideObject.layer, healthCheckLayerMask))
        {
            OnDamage(collideObject.GetComponent<Bullet>().damage);
        }
        else OnDamage(entityHealth);

        if (entityHealth <= 0f) FireBullet();
    }
}