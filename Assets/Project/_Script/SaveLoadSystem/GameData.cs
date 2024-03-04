using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class GameData
{
	#region Fields & Properties
	public Keyboard Keyboard;

	#endregion

	#region Methods 

	public GameData()
	{
		Keyboard = new Keyboard();
	}

	public void ClearData()
	{

	}

	#endregion
}