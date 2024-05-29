using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character4 : Character
{
	[Header("_~* 	Character 4 Unique stuff")]
	[SerializeField] protected Vector3 WallDimension;
	[SerializeField] protected float wallHP;
	[SerializeField] protected float wallDuration;
	[SerializeField] protected float wallCoolDown;
	bool canPlaceWall = true;

	new public static Character4 Create(Transform parent, Vector3 position)
	{
		Character4 character = Instantiate(Resources.Load<Character4>("_Prefabs/Characters/Character 4"), parent);
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
			if (!MyPet.IsDead)
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
		// if (Input.GetKeyDown(KeyCode.R))
		// {
		// 	myPet = Turret.Create();
		// 	myPet.transform.position = transform.position;
		// 	myPet.Initialize(this.tag);

		// 	Debug.Log("SUMMON Turret");
		// }

		if (Input.GetKey(KeyCode.R) && canPlaceWall)
		{
			StartCoroutine(PlaceWall());
		}
	}

	IEnumerator PlaceWall()
	{
		canPlaceWall = false;
		var wall = BulletproofWall.Create(WallDimension, wallHP, wallDuration, GetWorldMousePosition(), weapons[0].transform.rotation);
		wall.Initialize();
		wall.tag = this.tag;
		yield return new WaitForSeconds(wallCoolDown);
		canPlaceWall = true;
	}
}
