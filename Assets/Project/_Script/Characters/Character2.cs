using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character2 : Character
{
	#region Fields & Properties

	#endregion

	#region Methods
	new public static Character2 Create(Transform parent, Vector3 position)
	{
		Character2 character = Instantiate(Resources.Load<Character2>("_Prefabs/Characters/Character 2"), parent);
		character.transform.position = position;

		return character;
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public override void UpdateCharacter(List<Enemy> enemies = null)
	{
		base.UpdateCharacter();

		if (MyPet != null)
		{
			if(!MyPet.IsDead)
			{
				MyPet.UpdatePet(enemies);
			}
			else
			{
				Destroy(MyPet.gameObject);
				MyPet = null;
			}
		}
	}

	public override void KeyboardController()
	{
		base.KeyboardController();

		if (MyPet != null)
		{
			return;
		}

		// Summon pet
		if (Input.GetKeyDown(KeyCode.R))
		{
			MyPet = Turret.Create();
			MyPet.transform.position = transform.position;
			MyPet.Initialize(this.tag);

			Debug.Log("SUMMON Turret");
		}
	}


	#endregion
}

