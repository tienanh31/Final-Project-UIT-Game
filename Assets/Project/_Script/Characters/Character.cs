using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public class DicWeapon
//{
//	[SerializeField] public GameConfig.WEAPON _Type;
//	[SerializeField] public IWeapon _Weapon;
//}

public class Character : MonoBehaviour, IDamageable
{
	public static Character Instance { get; protected set; }

	#region Fields & Properties
	[Header("_~* 	Prefabs, Weapons & Stats")]
	[SerializeField] protected List<IWeapon> weapons;
	[SerializeField] protected SO_CharacterDefault soStats;
	[SerializeField] protected bool invulnerable = false;

	[Header("_~* 	User Interface")]
	[SerializeField] protected Canvas worldCanvas;
	[SerializeField] protected Canvas screenCanvas;
	[SerializeField] protected Pointer pointer;

	[SerializeField] protected TextMeshProUGUI worldText;
	[SerializeField] protected TextMeshProUGUI screenText;

	[Header("_~* 	Movement & control")]
	[SerializeField] protected Rigidbody characterRigidbody;
	[SerializeField] bool alignWithCamera = true;
	[SerializeField] float acceleration = 50f, deAcceleration = 50f, drag, dashForce = 50f, dashTime = 0.1f, dashCoolDown = 1f;

	bool _dashable = true;
	int currentWeapon = 0;

	public float AttackPriority { get; protected set; }
	public Pet MyPet { get; protected set; }
	public GameConfig.CHARACTER Type { get; protected set; }
	public bool IsDead { get; protected set; }
	public bool IsInPatrolScope { get; set; }

	public Action CollideEndingGate;

	protected float _HP;
	protected float _moveSpeed;
	protected float _skillCooldown;

	float speedX, speedZ;
	protected private Vector3 mousePos;
	Coroutine textCoroutine;
	bool movementEnable = true;

	private Action<float> _healthChange;
	private Action<int> _bulletChange;
	private Action<GameConfig.WEAPON> _weaponChange;

	protected Healthbar healthbar;

	#endregion

	#region Methods
	public static Character Create(Transform parent, Vector3 position)
	{
		Character character = Instantiate(Resources.Load<Character>("_Prefabs/Characters/Character"), parent);
		character.transform.position = position;
		
		return character;
	}

	public virtual void Initialize()
	{
		Instance = this;
		LevelManager.Instance.damageables.Add(this);
		healthbar = GetComponentInChildren<Healthbar>();
		characterRigidbody = GetComponent<Rigidbody>();
		pointer = GetComponentInChildren<Pointer>();

		//SO_Stats = GameManager.Instance.DataBank.weaponStats;
		//SO_CharacterDefault stats = GameManager.Instance.selectedCharacter.characterStats;

		_moveSpeed = soStats.MOVE_SPEED_DEFAULT;
		_HP = soStats.HP_DEFAULT;
		_skillCooldown = soStats.SKILL_COOLDOWN;

		InGame.Create();
		InGame inGameUI = UIManager.Instance.GetUI(UI.IN_GAME) as InGame;
		_healthChange = inGameUI.HealthChange;
		_bulletChange = inGameUI.BulletChange;
		_weaponChange = inGameUI.WeaponChange;

		foreach (IWeapon w in GetComponentsInChildren<IWeapon>())
		{
			//DicWeapon dw = new DicWeapon();
			//dw._Type = w.Type;
			//dw._Weapon = w;
			w.Initialize();
			w.tag = this.tag;
			w.source = this.gameObject;
			w.BulletChange = BulletWeaponChange;
			weapons.Add(w);
		}
		_weaponChange?.Invoke(weapons[0].Type);

		IsDead = false;

		speedX = 0;
		speedZ = 0;
		//characterRigidbody.drag = drag;
		healthbar.Start();


		// bonus
		var statBonus = GameManager.Instance.PlayerBonusStat;
		_HP += statBonus.HP;
		_moveSpeed += statBonus.MOVE_SPEED;
		foreach (var weapon in weapons)
        {
			weapon.AddDamageBonus(statBonus.ATTACK_BONUS);
        }

		_healthChange?.Invoke(_HP);

		InitEffect();
	}

	public virtual void UpdateUI()
	{
		worldCanvas.transform.LookAt(transform.position + Camera.main.transform.forward);
	}

	public virtual void UpdateCharacter(List<Enemy> enemies = null)
	{
		KeyboardController();
		//if (Input.GetKeyDown((KeyCode)DataPersistenceManager.Instance.GameData.Keyboard.Keyboards[KeyboardHandler.Dash]))
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (_dashable)
			{
				StartCoroutine(Dash());
			}				
		}
		MouseController();
		UpdateUI();
	}

	public virtual void KeyboardController()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (LevelManager.Instance.GamePaused)
			{
				UIManager.Instance.RemoveAllUIInPlayGame();
				LevelManager.Instance.ResumeGame();
			}
			else
			{
				PauseMenu.Create();
				LevelManager.Instance.PauseGame();
			}
		}
		if (!LevelManager.Instance.GamePaused)
		{
			CharacterMovement();
			SwapWeapon();
		}
	}

	public void MouseController()
	{
		if (!LevelManager.Instance.GamePaused)
		{
			mousePos = GetWorldMousePosition();
			RotateWeapon();

			if (Input.GetMouseButton(0))
			{
				weapons[currentWeapon].AttemptAttack();
			}
		}
	}

	public void TakenDamage(Damage damage)
	{
		if (_HP > 0 && !invulnerable)
		{
			_HP -= damage.value;
			healthbar.HealthUpdate();

			if (_HP < 0)
            {
				_healthChange?.Invoke(0);
            }
			else
            {
				_healthChange?.Invoke(_HP);
			}
			
			if (_HP <= 0)
			{
				Debug.Log("Character die");
				OnDeath();
			}
		}
	}

	public void TakenDamage(float damage)
	{
		if (_HP > 0 && !invulnerable)
		{
			_HP -= damage;
			healthbar.HealthUpdate();

			if (_HP < 0)
			{
				_healthChange?.Invoke(0);
			}
			else
			{
				_healthChange?.Invoke(_HP);
			}

			if (_HP <= 0)
			{
				Debug.Log("Character die");
				OnDeath();
			}
		}
	}

	#region Effect
	[SerializeField] ParticleSystem _stunEffect;
	[HideInInspector] public bool IsSlow = false;

	private void InitEffect()
    {
		_stunEffect = Instantiate<ParticleSystem>(_stunEffect);
		_stunEffect.transform.position = transform.position;
		_stunEffect.transform.SetParent(transform);
	}

    public void TakenStunEffect(float time)
    {
		Debug.Log("stun");	

		_stunEffect.gameObject.SetActive(true);
		var main = _stunEffect.main;
		main.duration = time;
		_stunEffect.Play();

		StartCoroutine(IE_Stun(time));
    }

	IEnumerator IE_Stun(float time)
    {
		movementEnable = false;
		yield return new WaitForSeconds(time);
		movementEnable = true;

		_stunEffect.gameObject.SetActive(false);
    }

	public void TakenSlowEffect(float ratio)
	{
		IsSlow = true;
		StartCoroutine(IE_Slow(ratio));
	}

	IEnumerator IE_Slow(float ratio)
    {
		var originSpeed = _moveSpeed;

		_moveSpeed -= _moveSpeed * ratio;
		while (IsSlow)
        {
			yield return null;
        }

		_moveSpeed = originSpeed;
		Debug.LogWarning("IE_Slow end");
	}

	public void Repel(Vector3 force)
    {
		Debug.Log("repel");
		characterRigidbody.AddForce(force, ForceMode.Impulse);
    }

	public void Falling(float force = 200)
    {
		characterRigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
	}
    #endregion
    public void TakenBuff(GameConfig.BUFF buff, float statBuff)
	{
		switch(buff)
		{
			case GameConfig.BUFF.HP:
				if (_HP < soStats.HP_DEFAULT)
				{
					SetWorldText($"Picked up {(statBuff).ToString()} HP!");
					_HP += statBuff;
					_HP = Mathf.Clamp(_HP, 0, soStats.HP_DEFAULT);

					_healthChange?.Invoke(_HP);
				}
				break;

			case GameConfig.BUFF.ATTACK:

				break;
		}
	}

	public void TakenWeapon(IWeapon weapon)
	{
		if (weapons.Any(x => x.name == weapon.name))
		{
			SetWorldText("You already have this weapon!", 3f);
		} else
		if (weapons.Count >= 2)
		{
			SetWorldText("All weapon slot are full!", 3f);
		} else
		{
			SetWorldText($"Picked up a {weapon.GetType().ToString()}!", 3f);
			weapon.gameObject.transform.SetParent(transform);
			weapon.Initialize();
			weapon.tag = this.tag;
			weapon.transform.position = weapons[0].transform.position;
			weapon.BulletChange = BulletWeaponChange;
			weapons.Add(weapon);
		}
	}

	public void BulletWeaponChange(int quantity) => _bulletChange.Invoke(quantity);

	private void CharacterMovement()
	{
		Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 movementInputAligned = movementInput.normalized;
		if (alignWithCamera)
		{
			Vector3 camToPlayer = this.transform.position - Camera.main.transform.position;
			camToPlayer.y = 0;
			movementInputAligned = Quaternion.LookRotation(camToPlayer.normalized) * movementInputAligned;
		}

		if (movementEnable)
		{
			if (Input.GetAxisRaw("Horizontal") != 0)
			{   //Use deAcceleration when changing moving direction (currently disabled)
				// if (speedX / Input.GetAxisRaw("Horizontal") > 0)
				// 	speedX += acceleration * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
				// else
				// 	speedX += deAcceleration * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
				speedX += acceleration * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
				speedX = Mathf.Clamp(speedX, -_moveSpeed * Mathf.Abs(movementInput.x), _moveSpeed * Mathf.Abs(movementInput.x));
			}
			else
				speedX = Mathf.MoveTowards(speedX, 0, deAcceleration * Time.deltaTime);
			if (Input.GetAxisRaw("Vertical") != 0)
			{   //Use deAcceleration when changing moving direction (currently disabled)
				// if (speedX / Input.GetAxisRaw("Horizontal") > 0)
				// 	speedZ += acceleration * Input.GetAxisRaw("Vertical") * Time.deltaTime;
				// else
				// 	speedZ += deAcceleration * Input.GetAxisRaw("Vertical") * Time.deltaTime;
				speedZ += acceleration * Input.GetAxisRaw("Vertical") * Time.deltaTime;
				speedZ = Mathf.Clamp(speedZ, -_moveSpeed * Mathf.Abs(movementInput.z), _moveSpeed * Mathf.Abs(movementInput.z));
			}
			else
				speedZ = Mathf.MoveTowards(speedZ, 0, deAcceleration * Time.deltaTime);
		}
		else
		{
			speedX = Mathf.MoveTowards(speedX, 0, deAcceleration * Time.deltaTime);
			speedZ = Mathf.MoveTowards(speedZ, 0, deAcceleration * Time.deltaTime);
		}

		Vector3 desiredMovement = new Vector3(speedX, 0, speedZ);
		if (alignWithCamera)
		{
			Vector3 camToPlayer = this.transform.position - Camera.main.transform.position;
			camToPlayer.y = 0;
			desiredMovement = Quaternion.LookRotation(camToPlayer.normalized) * desiredMovement;
		}
		//Debug.Log(desiredMovement + ", " + Input.GetAxisRaw("Horizontal") + ", " + Input.GetAxisRaw("Vertical"));
		
		Vector3 velocity = characterRigidbody.velocity;
		velocity.y = 0;
		float diffX = desiredMovement.x - velocity.x;
		float diffz = desiredMovement.z - velocity.z;

		//if theres no input or moving faster in same direction as input, no movement is apply, only friction
		diffX = Mathf.Clamp(diffX, -Mathf.Abs(desiredMovement.x), Mathf.Abs(desiredMovement.x));
		diffz = Mathf.Clamp(diffz, -Mathf.Abs(desiredMovement.z), Mathf.Abs(desiredMovement.z));
		
		Vector3 movement = new Vector3(diffX, 0, diffz);
		
		characterRigidbody.velocity += movement;

		//Debug.Log("Velocity: " + characterRigidbody.velocity + " | desiredMovement : " + desiredMovement + " | movement: " + movement);
	}

	public virtual void OnDeath()
	{
		LevelManager.Instance.damageables.Remove(this);
		StopAllCoroutines();
		IsDead = true;
	}

	public virtual void SwapWeapon()
	{
		// Main Weapon
		//if(Input.GetKeyDown((KeyCode)DataPersistenceManager.Instance.GameData.Keyboard.Keyboards[KeyboardHandler.Weapon1]))
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			if (currentWeapon != 0)
			{
				currentWeapon = 0;
				Debug.Log("Change to main weapon");
				_weaponChange?.Invoke(weapons[currentWeapon].Type);
				BulletWeaponChange(weapons[currentWeapon].GetCurrentBullet);
				weapons[currentWeapon].OnSwapTo();
			}
		}

		// Grenade
		//if (Input.GetKeyDown((KeyCode)DataPersistenceManager.Instance.GameData.Keyboard.Keyboards[KeyboardHandler.Weapon2]))
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if(weapons.Count > 1 && currentWeapon != 1)
			{
				currentWeapon = 1;
				Debug.Log("Change to second weapon");
				_weaponChange?.Invoke(weapons[currentWeapon].Type);
				BulletWeaponChange(weapons[currentWeapon].GetCurrentBullet);
				weapons[currentWeapon].OnSwapTo();
			}
		}
	}

	public void AddForce(Vector3 direction)
	{
		//characterRigidbody.velocity = Vector3.zero;
		speedX = 0; speedZ = 0;
		characterRigidbody.AddForce(direction, ForceMode.Impulse);
	}

	IEnumerator Dash()
	{
		StartCoroutine(IE_DashCoolDown());

		Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		if (alignWithCamera)
		{
			Vector3 camToPlayer = this.transform.position - Camera.main.transform.position;
			camToPlayer.y = 0;
			movementInput = Quaternion.LookRotation(camToPlayer) * movementInput;
		}
		movementInput = movementInput.normalized;

		characterRigidbody.velocity = Vector3.zero;
		speedX = 0; speedZ = 0;
		characterRigidbody.AddForce(movementInput.normalized * dashForce, ForceMode.Impulse);
		movementEnable = false;
		yield return new WaitForSeconds(dashTime);
		movementEnable = true;
	}

	IEnumerator IE_DashCoolDown()
    {
		_dashable = false;
		yield return new WaitForSeconds(dashCoolDown);
		_dashable = true;
    }

	private void RotateWeapon()
	{
		Vector3 location = mousePos;
		var q = Quaternion.LookRotation(location - transform.position);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 1000f * Time.deltaTime);
	}

	public Vector3 GetWorldMousePosition()
	{
		Plane plane = new Plane(Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		plane.Raycast(ray, out float distance);
		return ray.GetPoint(distance);
	}

	public void SetWorldText(string t, float time = 1f)
	{
		if (textCoroutine != null)
			StopCoroutine(textCoroutine);
		textCoroutine = StartCoroutine(WorldTextHandle(t, time));
	}

	private IEnumerator WorldTextHandle(string str, float time)
	{
		worldText.SetText(str);
		yield return new WaitForSeconds(time);
		worldText.SetText("");
	}

	public void SetScreenText(string t)
	{
		screenText.SetText(t);
	}

	public virtual float GetHP()
	{
		return _HP;
	}

	public void SetPointerTarget(Transform transform)
	{
		if (pointer != null)
		{
			pointer.SetTarget(transform);
		}
	}

	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.name == "EndingGate")
        {
			Debug.LogWarning("complete");
			CollideEndingGate?.Invoke();

		}
    }

	[ExecuteInEditMode]
	private void ApplyChanges()
	{
		foreach (IWeapon w in GetComponentsInChildren<IWeapon>())
		{
			//DicWeapon dw = new DicWeapon();
			//dw._Type = w.Type;
			//dw._Weapon = w;
			weapons.Add(w);
		}
	}
	#endregion
}
