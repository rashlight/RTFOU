using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BossCheetah))]
public class LastWord_BossCheetah : EntityPattern {
    [SerializeField] private BossCheetah bossObject;
    [SerializeField] private GameObject firingShip;
    [SerializeField] private int summonTime;
    private Coroutine timerCoroutine;

    public override IEnumerator StartSequence()
    {
        bossObject.isAttacking = false;
        timerCoroutine = StartCoroutine(SetTimer());
        yield return StartCoroutine(Summon(sequenceDelay));
    }

    public IEnumerator SetTimer()
    {
        while (true)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(sequenceDelay);
            GlobalVar.self.bossText.text = "(LW) " + timeSpan.ToString("mm':'ss'.'f");
            yield return new WaitForEndOfFrame();
        }  
    }

    public IEnumerator Summon(float time)
    {
        float timePerSummon = time / summonTime;
        float timerCollective = 0f;
        
        while (sequenceDelay > 0f)
        {
            if (timerCollective >= timePerSummon)
            {
                for (int i = 0; i < Mathf.FloorToInt(timerCollective / timePerSummon); i++)
                {
                    GameObject summoner = bossObject.bossSummoners[Random.Range(0, bossObject.bossSummoners.Length)];
                    Instantiate(firingShip, summoner.transform.position, Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)));
                    timerCollective -= timePerSummon;
                }
            }
            timerCollective += Time.deltaTime;
            sequenceDelay = Mathf.Clamp(sequenceDelay - Time.deltaTime, 0f, float.MaxValue);
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine(timerCoroutine);
        yield return null;
    }
}