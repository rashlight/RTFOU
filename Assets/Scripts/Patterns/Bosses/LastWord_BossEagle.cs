using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossEagle))]
public class LastWord_BossEagle : EntityPattern {
    [SerializeField] private BossEagle bossObject;
    [SerializeField] private GameObject firingShip;
    private Coroutine timerCoroutine;

    public override IEnumerator StartSequence()
    {
        bossObject.isAttacking = false;
        timerCoroutine = StartCoroutine(SetTimer());
        yield return StartCoroutine(Summon(sequenceDelay));
    }

    public IEnumerator SetTimer()
    {
        float timer = sequenceDelay;
        while (true)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timer);
            GlobalVar.self.bossText.text = "(LW) " + timeSpan.ToString("mm':'ss'.'f");
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }  
    }

    public IEnumerator Summon(float time)
    {
        List<EntityInfo> enemyList = new List<EntityInfo>();
        for (int i = 0; i < bossObject.bossSummoners.Length; i++)
        {
            GameObject summoner = bossObject.bossSummoners[i];
            enemyList.Add(Instantiate(firingShip, summoner.transform.position, Quaternion.identity).GetComponent<EntityInfo>());
        }
        yield return new WaitForSeconds(time);
        foreach (EntityInfo obj in enemyList)
        {
            if (obj != null) obj.OnDeath();
        }
        StopCoroutine(timerCoroutine);
        yield return null;
    }
}