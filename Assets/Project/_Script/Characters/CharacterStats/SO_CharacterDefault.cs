using UnityEngine;

[CreateAssetMenu(menuName = "Character Data/Character Default Stats")]
public class SO_CharacterDefault : ScriptableObject
{
    public float HP_DEFAULT = 150;
    public float MOVE_SPEED_DEFAULT = 6;
    public float ATTACK_BONUS = 0f;
    public float SKILL_COOLDOWN = 1;
}

