using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BossCheetah))]
public class Intro_BossCheetah : EntityPattern {
    [SerializeField] private BossCheetah bossObject;
    [SerializeField] private Vector3 translateVector;

    public override IEnumerator StartSequence()
    {
        yield return StartCoroutine(TranslateBoss(sequenceDelay));
    }

    public IEnumerator TranslateBoss(float time)
    {
        float bossHealth = bossObject.entityHealth;
        bossObject.entityHealth = float.PositiveInfinity;

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
            transform.position = Vector3.Lerp(beginVector, finishedVector, percentage);
            System.TimeSpan lerpStopTime = System.TimeSpan.FromSeconds(bossObject.stopTime * percentage);
            GlobalVar.self.bossText.text = Mathf.Round(bossHealth * percentage) + " \\ " + lerpStopTime.ToString("mm':'ss'.'f");
            // Changes difficulty color to that of the boss
            Color defaultMultiplierTextColor = GlobalProc.self.gameMultiplierText.color;
            GlobalProc.self.gameMultiplierText.color = new Color (
                Mathf.Lerp(defaultMultiplierTextColor.r, bossObject.bossColor.r, percentage),
                Mathf.Lerp(defaultMultiplierTextColor.g, bossObject.bossColor.g, percentage),
                Mathf.Lerp(defaultMultiplierTextColor.b, bossObject.bossColor.b, percentage),
                Mathf.Lerp(defaultMultiplierTextColor.a, bossObject.bossColor.a, percentage)
            );
            yield return new WaitForEndOfFrame();
        }

        // Set boss health to default value
        bossObject.entityHealth = bossHealth;

        // Set a random weapon to the player
        GlobalVar.self.player.playerWeapons[bossObject.selectedGunIndex].gameObject.SetActive(true);

        // Changes boss information
        bossObject.SetBossText();

        bossObject.ApplyExtraJump();
        yield return null;
    }
}