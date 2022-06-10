using System.Collections;
using UnityEngine;

public class RandomSummon : EntityPattern {

    [SerializeField] private GameObject[] summonObjects;
    [SerializeField] private BulletSummoner[] bulletSummoners;
    [SerializeField] private int summonAmount;

    private void Start() {
        summonAmount *= Mathf.FloorToInt(GlobalVar.GameDifficulty);
    }

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(SummonRandom());
    }

    IEnumerator SummonRandom()
    {
        GameController gc = GlobalVar.self.GetComponent<GameController>();

        for (int i = 0; i <= summonAmount; i++)
        {
            BulletSummoner summoner = bulletSummoners[Random.Range(0, bulletSummoners.Length)];
            summoner.summonBullet = summonObjects[Random.Range(0, bulletSummoners.Length)];
            Vector3 bulletEulerAngle = summoner.summonBullet.transform.eulerAngles;
            summoner.summonBullet.transform.eulerAngles = new Vector3(
                bulletEulerAngle.x,
                bulletEulerAngle.y,
                Random.Range(-20f, 20f)
            );
            summoner.Summon();
            yield return new WaitForSeconds(sequenceDelay / summonAmount * (1f / GlobalVar.GameDifficulty));
        }

        yield return null;
    }
}