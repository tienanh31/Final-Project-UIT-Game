using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameConfig
{
	public const float TIME_STOP_AFTER_PATROLLING = 2f;

	public const float RATIO_DROP_BUFF = 0.5f;
	public const float RATIO_DROP_ITEM = 0.0f;

	public enum COLLIDABLE_OBJECT
	{
		ENEMY = 0,
		OBSTACLE
	}

	public enum SO_TYPE
	{
		CHARACTER = 0,
		WEAPON,
		ENEMY,
		PET
	}

	public enum STAT_TYPE
	{
		HP,
		ATTACK_SPEED,
		DAMAGE,
		MOVE_SPEED,
		ATTACK_RANGE,
		EFFECT_SCOPE,
		BULLET_SPEED,
		DETECT_RANGE
	}

	public enum WEAPON
	{
		RIFLE = 0,
		GERNADETHROWER = 1,
		MINE_PRODUCER = 2,
		BULLETPROOF_WALL = 3,
		SHOTGUN = 4,
		SWORD = 5,
		SNIPER
	}

	public enum CHARACTER
	{
		CHARACTER_DEFAULT = 0,
		CHARACTER_1 = 1,
		CHARACTER_2 = 2,
		CHARACTER_3 = 3,
		CHARACTER_4 = 4
	}

	public enum ENEMY
	{
		ENEMY_MELEE = 0,
		ENEMY_DEFAULT,
		ENEMY_SHIELD,
		ENEMY_SUICIDE,
		ENEMY_ARTILERY,
		TRAP,
		ENEMY_TURRET,
		ENEMY_NIGHTTIME,
	}

	public enum PET
	{ 
		DRONE = 0,
		TURRET = 1
	}

	public enum PICKABLE_TYPE
	{
		BUFF = 0,
		ITEM,
	}

	public enum BUFF
	{
		HP = 0,
		ATTACK,
	}
}

