using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class GameData
{
	#region Fields & Properties
	public int Level;

	public VictoryScreen.BuffStat PlayerBuffStat;

	public VictoryScreen.BuffStat EnemiesBuffStat;

	#endregion

	#region Methods 

	public GameData()
	{
		Level = 1;
		PlayerBuffStat = new VictoryScreen.BuffStat()
		{
			HP = 0,
			MOVE_SPEED = 0,
			ATTACK_BONUS = 0,
		};
		EnemiesBuffStat = new VictoryScreen.BuffStat()
		{
			HP = 0,
			MOVE_SPEED = 0,
			ATTACK_BONUS = 0,
		};
	}

	public void SetData(int level, VictoryScreen.BuffStat player, VictoryScreen.BuffStat enemies)
    {
		Level = level;
		PlayerBuffStat = player;
		EnemiesBuffStat = enemies;
    }

	public void ClearData()
	{

	}

	#endregion
}