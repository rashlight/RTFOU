using System.Collections;
using UnityEngine;

public class EagleSummon : EntityPattern {
    
    [SerializeField]
    private GameObject[] summonObjects;
    [SerializeField]
    private float spawnChance = 33f;

    private void Start() {
        spawnChance *= Mathf.FloorToInt(GlobalVar.GameDifficulty);
    }

    public override IEnumerator StartSequence()
    {
        if (Random.Range(1f, 100f) > spawnChance) yield return null;
        else yield return StartCoroutine(SummonEagle());
    }

    IEnumerator SummonEagle()
    {
        GameController gc = GlobalVar.self.GetComponent<GameController>();
        BulletSummoner summoner = gc.summonPivots[Random.Range(0, gc.summonPivots.Length)].GetComponent<BulletSummoner>();
        summoner.summonBullet = summonObjects[Random.Range(0, summonObjects.Length)];
        summoner.Summon();
        yield return null;
    }
}