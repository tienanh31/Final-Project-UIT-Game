using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character1 : Character
{
	#region Fields & Properties
	[Header("_~* 	Character 4 Unique stuff")]
	[SerializeField] protected Drone drone;
	[SerializeField] protected float skillCooldown, droneThrowForce;
	bool canSummonDrone = true;

	#endregion

	#region Methods
	new public static Character1 Create(Transform parent, Vector3 position)
	{
		Character1 character = Instantiate(Resources.Load<Character1>("_Prefabs/Characters/Character 1"), parent);
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

		if(MyPet != null)
		{
			MyPet.UpdatePet(enemies);
		}
	}

	public override void KeyboardController()
	{
		base.KeyboardController();

		if (MyPet != null)
		{
			//return;
		}

		if (Input.GetKey(KeyCode.R) && canSummonDrone)
		{
			StartCoroutine(SummonDrone());
		}
	}

	IEnumerator SummonDrone()
	{
		canSummonDrone = false;
		Drone drone = Drone.Create(this.transform.position, this.transform.rotation);
		drone.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * droneThrowForce, ForceMode.Impulse);
		yield return new WaitForSeconds(skillCooldown);
		canSummonDrone = true;
	}

	#endregion
}

