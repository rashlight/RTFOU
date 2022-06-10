using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all entities in game, e.g. health, name, events, ...
/// </summary>
public abstract class EntityInfo : MonoBehaviour
{
    public string entityName;
    public float entityHealth;
    [Tooltip("Check this if you want to implement your own OnDeath() system")]
    public bool isCheckDeath = true;
    /// <summary>
    /// Prevents OnDeath() from being called more than once.
    /// </summary>
    private bool isOnDeathCalled = false;

    public virtual void OnDamage(float amount)
    {
        entityHealth -= amount;
        if (isCheckDeath && !isOnDeathCalled && entityHealth <= 0f) 
        {
            OnDeath();
            // There should be only one OnDeath() call for each EntityInfo object.
            isOnDeathCalled = true;
        }
    }
    public virtual void OnHeal(float amount)
    {
        entityHealth += amount;
    }    
    public virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
