using System.Collections;
using UnityEngine;

public class ShipPattern_BossAny : EntityPattern {
    [SerializeField] private BossBullet bossObject;
    [SerializeField] private GameObject firingShip;
    [SerializeField] private int summonTime;
    [SerializeField] private bool isAffectedByDifficulty = false;

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(Summon(sequenceDelay, Mathf.FloorToInt(summonTime * (GlobalVar.GameDifficulty - 1f))));
    }

    public IEnumerator Summon(float time, int amount)
    {
        float timePerSummon = time / summonTime;
        float timerCollective = 0f;
        int objectSummoned = 0;

        while (objectSummoned < summonTime)
        {
            if (timerCollective >= timePerSummon)
            {
                for (int i = 0; i < Mathf.FloorToInt(timerCollective / timePerSummon); i++)
                {
                    GameObject summoner = bossObject.bossSummoners[Random.Range(0, bossObject.bossSummoners.Length)];
                    Instantiate(firingShip, summoner.transform.position, summoner.transform.rotation);
                    timerCollective -= timePerSummon;
                    objectSummoned++;
                    if (objectSummoned >= summonTime) break;
                }
            }
            timerCollective += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}