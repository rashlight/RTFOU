using System.Collections;
using UnityEngine;

public class RandomBullet : Bullet {

    [SerializeField] private EntityPattern introPattern;
    [SerializeField] private EntityPattern outroPattern;
    [SerializeField] private EntityPattern[] firingPatterns;
    
    protected override void OnStart() 
    {
        StartCoroutine(EventManager());
    }

    protected override void OnUpdate()
    {
        if (entityHealth <= 0f) StopAllCoroutines();
    }

    IEnumerator EventManager()
    {
        yield return StartCoroutine(introPattern.StartSequence());
        yield return StartCoroutine(Summon());
        yield return null;
    }

    IEnumerator Summon()
    {
        yield return StartCoroutine(firingPatterns[Random.Range(0, firingPatterns.Length)].StartSequence());
        yield return StartCoroutine(outroPattern.StartSequence());
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