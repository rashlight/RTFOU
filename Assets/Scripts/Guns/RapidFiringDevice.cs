using UnityEngine;

public class RapidFiringDevice : Gun {

    public float damageControlRatio = 1f;
    private int damageClamp = 0;

    public override void OnStart() {
        damageClamp = Mathf.RoundToInt(magazineSize * damageControlRatio);
    }

    public override void OnFire()
    {
        base.OnFire();
        damage = Mathf.Clamp(damage + 1, 0, damageClamp);
        UpdateGunStatus();
    }

    public override void OnOutOfAmmo()
    {
        base.OnOutOfAmmo();
        magazineSize += 2;
        damage = 1;
        UpdateGunStatus();
    }

    public override void OnAmmoRechargeFinished()
    {
        damageClamp = Mathf.RoundToInt(magazineSize * damageControlRatio);
        base.OnAmmoRechargeFinished();
    }
}