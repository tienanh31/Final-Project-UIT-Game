using UnityEngine;

[CreateAssetMenu(menuName = "Pet Data/Drone Stats")]
public class SO_Drone: ScriptableObject
{
    public float HP_DEFAULT = 150;

    public float DAMAGE_DEFAULT = 20;

    public float ATTACK_RANGE_DEFAULT = 2;
    public float ATTACK_SPEED_DEFAULT = 1; // x times per second 

    public float BULLET_SPEED = 3;

    public float MOVE_SPEED_DEFAULT = 6;
}

