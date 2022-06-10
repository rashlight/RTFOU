using System.Collections;
using UnityEngine;

public class LaserSummon : EntityPattern {
    [SerializeField] private GameObject[] summonObjects;
    [SerializeField] private BulletSummoner[] bulletSummoners;
    [SerializeField] private int summonAmount;
    [SerializeField] private int summonMaxLength;

    private void Start() {
        sequenceDelay *= Mathf.FloorToInt(Mathf.Clamp(GlobalVar.GameDifficulty, 1f, float.MaxValue));
        summonAmount *= Mathf.FloorToInt(Mathf.Clamp(GlobalVar.GameDifficulty / 3f, 1f, float.MaxValue));
        summonMaxLength *= Mathf.FloorToInt(Mathf.Clamp(GlobalVar.GameDifficulty, 1f, float.MaxValue));
    }

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(SummonLaser());
    }

    IEnumerator SummonLaser()
    {
        for (int i = 0; i < summonAmount; i++)
        {
            yield return StartCoroutine(SummonSingleLaser());
        }

        yield return null;
    }

    IEnumerator SummonSingleLaser()
    {
        GameObject bullet = summonObjects[Random.Range(0, summonObjects.Length)];

        BulletSummoner summoner = bulletSummoners[Random.Range(0, bulletSummoners.Length)];
        summoner.summonBullet = bullet;      

        for (int i = 0; i < summonMaxLength; i++)
        {
            summoner.Summon();
            yield return new WaitForSeconds(sequenceDelay / summonAmount / summonMaxLength * (1 / GlobalVar.GameDifficulty));
        }

        yield return null;
    }
}