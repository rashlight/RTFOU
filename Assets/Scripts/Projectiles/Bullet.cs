using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a projectile that can collides with objects
/// and does actions with it.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : EntityInfo
{
    [Tooltip("It is highly recommended to use natural numbers when possible.")]
    public float damage = 10f;
    public float speed = 3f;
    public float despawnDelay = 1f;
    public SpriteRenderer spriteRenderer;
    [Tooltip("Never put non-bullet entities in this array!")]
    public List<LayerMask> healthCheckLayerMask;
    protected bool isRendered = false;
    protected Player playerHandler;
    [SerializeField] protected Rigidbody2D bulletRigidbody;
    [SerializeField] protected Collider2D bulletCollider;
    [SerializeField] protected List<string> includedDamageTags;
    
    protected bool CheckLayerMask(LayerMask collideLayerMask, List<LayerMask> contactLayerList)
    {      
        foreach (var mask in contactLayerList)
        {
            if (Mathf.Pow(2, collideLayerMask.value) == mask.value) return true;
        }
        return false;
    }

    private void CheckDespawnDelay()
    {
        if (!spriteRenderer.isVisible)
        {
            despawnDelay -= Time.deltaTime;
            if (despawnDelay <= 0f) OnDeath();
        }
    }

    protected virtual void OnStart()
    {
        
    }

    protected virtual void OnUpdate()
    {

    }

    private void Start() {
        playerHandler = GlobalVar.self.player;
        OnStart();
    }

    // Called every frame
    private void Update()
    {
        // Check if player is destroyed (death)
        if (playerHandler == null) 
        {
            Destroy(gameObject);
        }
        OnUpdate();
        CheckDespawnDelay();
    }
}
