using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Data/Thrower Stats")]
public class SO_WeaponThrowerStats : ScriptableObject
{
    public float DAMAGE_DEFAULT = 20;
    public float DELAY_BETWEEN_THROW = 10;
    public float COOLDOWN = 1;
    public float MAX_RANGE = 3;
    public int MAX_GERNADE_COUNT = 3;
    public AudioClip throwSFX;
}

