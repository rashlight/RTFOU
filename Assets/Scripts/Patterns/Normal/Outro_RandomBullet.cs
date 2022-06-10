using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RandomBullet))]
public class Outro_RandomBullet : EntityPattern {
    [SerializeField] private RandomBullet bulletObject;
    [SerializeField] private Vector3 translateVector;

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(TranslateBoss(sequenceDelay));
    }

    public IEnumerator TranslateBoss(float time)
    {
        Vector3 beginVector = transform.position;
        Vector3 finishedVector = beginVector + translateVector;
        float percentage = 0f;
        float totalTranslateTime = 0f;

        // Base loop
        while (percentage < 1f)
        {
            if (GlobalVar.self.player == null) yield return null;
            totalTranslateTime += Time.deltaTime;
            percentage = totalTranslateTime / time;
            transform.position = Vector3.Slerp(beginVector, finishedVector, percentage);
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }
}