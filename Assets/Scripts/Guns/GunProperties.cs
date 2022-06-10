public class GunProperties {
    public float damage;
    public float fireRate;
    public float spread;
    public int magazineSize;
    public float reloadRate;
    public int bulletAmount;
    public bool isReloadMode = false;
    public bool isPulledTrigger = false;

    public GunProperties(Gun gun)
    {
        damage = gun.damage;
        fireRate = gun.fireRate;
        spread = gun.spread;
        magazineSize = gun.magazineSize;
        reloadRate = gun.reloadRate;
        bulletAmount = gun.bulletAmount;
        isReloadMode = gun.isReloadMode;
        isPulledTrigger = gun.isPulledTrigger;
    }

    ~GunProperties()
    {

    }

    public void SetProperties(Gun gun)
    {
        gun.damage = damage;
        gun.fireRate = fireRate;
        gun.spread = spread;
        gun.magazineSize = magazineSize;
        gun.reloadRate = reloadRate;
        gun.bulletAmount = magazineSize;
        gun.isReloadMode = false;
        gun.isPulledTrigger = false;
    }
}