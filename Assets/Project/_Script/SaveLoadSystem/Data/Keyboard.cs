using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]

public class Keyboard
{
	#region Fields & Properties

	public Dictionary<KeyboardHandler, int> Keyboards;

	#endregion

	#region Methods 

	public Keyboard()
	{
		Keyboards = new Dictionary<KeyboardHandler, int>();
		Keyboards.Add(KeyboardHandler.MoveForward, (int)KeyCode.W);
		Keyboards.Add(KeyboardHandler.MoveBack, (int)KeyCode.S);
		Keyboards.Add(KeyboardHandler.MoveLeft, (int)KeyCode.A);
		Keyboards.Add(KeyboardHandler.MoveRight, (int)KeyCode.D);
		Keyboards.Add(KeyboardHandler.Dash, (int)KeyCode.Space);
		Keyboards.Add(KeyboardHandler.Weapon1, (int)KeyCode.Alpha1);
		Keyboards.Add(KeyboardHandler.Weapon2, (int)KeyCode.Alpha2);
	}

	#endregion
}

