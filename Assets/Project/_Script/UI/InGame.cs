using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InGame: MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] RawImage _weaponUI;
    [SerializeField] TMP_Text _bullet;
    [SerializeField] TMP_Text _healthPoint;
    [SerializeField] List<Texture2D> _weaponImage;

    public UI Type { get; set; }
    #endregion

    public static InGame Create(Transform parent = null)
    {
        InGame inGame = Instantiate(Resources.Load<InGame>("_Prefabs/UI/InGame"), parent);
        inGame.Type = UI.IN_GAME;

        UIManager.Instance.UserInterfaces.Add(inGame);
        return inGame;
    }

    public void HealthChange(float healthPoint)
	{
        _healthPoint.text = healthPoint.ToString();
	}   
    
    public void BulletChange(int bullet)
	{
        _bullet.text = bullet.ToString();
	}

    public void WeaponChange(GameConfig.WEAPON weapon)
	{
        if (_weaponImage.Count > (int)weapon)
        {
            _weaponUI.texture = _weaponImage[(int)weapon];
        }
	}

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
