using UnityEngine;

public class HighFire : Gun {

    [Header("Non-inheritence definitions")]
    public int totalShot = 5;
    private bool isEventCalled = false;

    public override void OnFire()
    {
        // Prevents recursive event
        if (isEventCalled) return;
        isEventCalled = true;

        base.OnFire();

        // In case of smaller amount, exclusion doesn't apply, so bump up to 1
        int shotToFire = (bulletAmount < totalShot) ? bulletAmount + 1 : totalShot; 
        // The gun has already shot once, excluding the former occurence
        for (int i = 0; i < shotToFire - 1; i++) FireBullet();

        isEventCalled = false;
        UpdateGunStatus();
    }

    public override void OnOutOfAmmo()
    {
        // Prevents recursive event
        if (isEventCalled) return;
        
        base.OnOutOfAmmo();
        magazineSize += 3;
        damage += 2;
        totalShot = Mathf.Clamp(totalShot + 1, 0, 8);
        UpdateGunStatus();
    }
}