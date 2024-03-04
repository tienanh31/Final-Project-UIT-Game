using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Gun Stats")]
public class SO_WeaponGunStats : ScriptableObject
{
    public float DAMAGE_DEFAULT = 20;
    public float ATTACK_RANGE_DEFAULT = 10;
    public float ATTACK_SPEED_DEFAULT = 10; // x times per second 
    public float RELOAD_TIME = 1;
    public int MAGAZINE_CAPACITY = 30;
    public float INACCURACY = 0.01f;
    public float BULLET_SPEED = 10;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;
    public AudioClip doneReloadSFX;
}

