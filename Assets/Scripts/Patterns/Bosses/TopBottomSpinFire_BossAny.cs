using System.Collections;
using UnityEngine;

public class TopBottomSpinFire_BossAny : EntityPattern {
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
        GameObject summonerTop = bossObject.bossSummoners[0];
        GameObject summonerBottom = bossObject.bossSummoners[bossObject.bossSummoners.Length - 1];

        for (int i = 1; i <= summonTime / 2; i++)
        {
            // Top
            GameObject obj = Instantiate(firingShip, summonerTop.transform.position, summonerBottom.transform.rotation);
            obj.transform.rotation = Quaternion.Euler(
                0f, 
                0f, 
                startZPos + rotationPerCycle * (i - 1)
            );

            // Bottom
            obj = Instantiate(firingShip, summonerBottom.transform.position, summonerBottom.transform.rotation);
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