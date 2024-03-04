using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Option: MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] Slider _volume0;
    [SerializeField] Slider _volume1;
    [SerializeField] Slider _volume2;
    [SerializeField] Button _keyboard;
    [SerializeField] Button _back;

    public UI Type { get; set; }

    public UI PreviousUI { get; set; }
    #endregion

    public static Option Create(Transform parent = null)
    {
        Option option = Instantiate(Resources.Load<Option>("_Prefabs/UI/Option"), parent);
        option.Type = UI.OPTION;

        UIManager.Instance.UserInterfaces.Add(option);
        option.UpdateValue();
        return option;
    }

    private void OnEnable()
    {
        _keyboard.onClick.AddListener(KeyboardSetting);
        _back.onClick.AddListener(Back);

        UpdateValue();

        if (GameManager.Instance.IsInPlayScene())
        {
            _keyboard.enabled = false;
            Destroy(_keyboard.gameObject);
        }
    }

    public void UpdateValue()
    {
        _volume0.value = UIManager.Instance.GetMasterVolumn() / 5;
        _volume1.value = UIManager.Instance.GetMusicVolumn() / 5;
        _volume2.value = UIManager.Instance.GetSFXVolumn() / 5;

        Debug.Log(_volume0.value);
    }

    public void MasterVolumeChange()
    {
        UIManager.Instance.SetMasterValue(0 + (_volume0.value * 5));
    }

    public void MusicVolumeChange()
    {
        UIManager.Instance.SetMusicValue(0 + (_volume1.value * 5));
    }

    public void SFXVolumeChange()
    {
        UIManager.Instance.SetSFXValue(0 + (_volume2.value * 5));
    }

    private void KeyboardSetting()
	{
        KeyboardMenu.Create();
        Destroy(gameObject);
	}

    private void Back()
    {
        switch(PreviousUI)
		{
            case UI.MAIN_MENU:
                MainMenu.Create();
                break;

            case UI.PAUSE:
                PauseMenu.Create();
                break;
		}
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        _keyboard.onClick.RemoveListener(KeyboardSetting);
        _back.onClick.RemoveListener(Back);

        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
