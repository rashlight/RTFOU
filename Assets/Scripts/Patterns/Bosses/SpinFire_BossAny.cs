using System.Collections;
using UnityEngine;

public class SpinFire_BossAny : EntityPattern {
    [SerializeField] private BossBullet bossObject;
    [SerializeField] private GameObject firingShip;
    [SerializeField] private int summonTime;
    [SerializeField] private float startZPos;
    [SerializeField] private float endZPos;

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(Summon(sequenceDelay));
    }

    public IEnumerator Summon(float time)
    {
        float rotationPerCycle = (endZPos - startZPos) / summonTime;
        GameObject summoner = bossObject.bossSummoners[Random.Range(0, bossObject.bossSummoners.Length)];

        for (int i = 1; i <= summonTime; i++)
        {
            GameObject obj = Instantiate(firingShip, summoner.transform.position, summoner.transform.rotation);
            obj.transform.rotation = Quaternion.Euler(
                0f, 
                0f, 
                startZPos + rotationPerCycle * (i - 1)
            );
            yield return new WaitForSeconds(time / summonTime);
        }
        yield return null;
    }
}