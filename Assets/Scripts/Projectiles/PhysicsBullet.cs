using System.Collections.Generic;
using UnityEngine;
 
public class PhysicsBullet : Bullet {

    [Header("Non-inheritence definitions")]
    public bool isRandomLaunchAngle = true;
    public bool isGroundInteractable = false;
    public float startingForce = 250f;
    public Vector2 launchingVector = Vector2.up;
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
    }
}