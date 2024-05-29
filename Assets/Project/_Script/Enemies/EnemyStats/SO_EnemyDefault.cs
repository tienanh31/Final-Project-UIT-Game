using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data/Enemy Default Stats")]
public class SO_EnemyDefault: ScriptableObject
{
    public float HP_DEFAULT = 100f;
    public float ATTACK_RANGE_DEFAULT = 4f;
    public float MOVE_SPEED_DEFAULT = 6f;
    public float DETECT_RANGE = 6f;
    public float TURNING_SPEED = 180f;
}

