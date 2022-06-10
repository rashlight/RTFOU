using System.Collections;
using UnityEngine;

public class BossFrog : BossBullet {
    public float restTime = 0f;
    private bool isLastWordStarting = false;

    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(EventManager());
    }

    public override void OnFixedUpdate()
    {
        if (!isLastWordStarting && entityHealth <= 0f)
        {
            StopAllCoroutines();
            StartCoroutine(LastWord());
            isLastWordStarting = true;
        }
    }
    IEnumerator EventManager()
    {
        yield return StartCoroutine(introPattern.StartSequence());
        isAttacking = true;
        while (entityHealth > 0f)
        {
            yield return StartCoroutine(bossPatterns[Random.Range(0, bossPatterns.Length)].StartSequence());
            yield return new WaitForSeconds(restTime);
        };
        yield return null;
    }

    IEnumerator LastWord()
    {
        if (isLastWordEnabled) yield return StartCoroutine(lastWordPattern.StartSequence());
        GlobalProc.self.gameMultiplierText.color = GlobalVar.self.defaultTextColor;
        OnDeath();
        //Debug.Log("boss defected");
    }
}