using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Pandemonium/Weapon", order = 1)]
public class WeaponItem : Item
{

    public int damage;
    public bool isRangeWeapon;

    public float range;
    public Item ammo;
    public int magazineSize;
    public WeaponType type;

    public bool asBulletSpread = true;
    public Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    public GameObject ImpactParticuleSystem;
    public float ShootDelay = 0.5f;
    public TrailRenderer BulletTrail;
    public float forceMagnitude = 500f;
    public bool isAutomaticWeapon = false;
    public float bulletSpeed = 100;
}
