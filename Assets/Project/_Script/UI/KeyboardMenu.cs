using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum KeyboardHandler
{
    MoveForward = 0,
    MoveBack,
    MoveLeft,
    MoveRight,
    Weapon1,
    Weapon2,
    Dash
}

[System.Serializable]
public class DictionaryUIKeyboard
{
    public KeyboardHandler Type;
    public TMP_Dropdown Dropdown;
}

public class KeyboardMenu: MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] List<DictionaryUIKeyboard> _keyboard;
    [SerializeField] Button _back;
	[SerializeField] Button _save;

    public UI Type { get; set; }
    #endregion

    public static KeyboardMenu Create(Transform parent = null)
    {
		KeyboardMenu keyboard = Instantiate(Resources.Load<KeyboardMenu>("_Prefabs/UI/KeyboardMenuWithScrollView"), parent);
        keyboard.Type = UI.KEY;

        UIManager.Instance.UserInterfaces.Add(keyboard);
        return keyboard;
    }

    private void InitKeyboard()
	{
        foreach(var key in _keyboard)
		{
            key.Dropdown.ClearOptions();
		}

		_keyboard[0].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.W.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.UpArrow.ToString()),
		});

		_keyboard[1].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.S.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.DownArrow.ToString()),
		});

		_keyboard[2].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.A.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.LeftArrow.ToString()),
		});

		_keyboard[3].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.D.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.RightArrow.ToString()),
		});

		_keyboard[4].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.Alpha0.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha1.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha2.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha3.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha4.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha5.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha6.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha7.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha8.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha9.ToString()),
		});

		_keyboard[5].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.Alpha0.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha1.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha2.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha3.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha4.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha5.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha6.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha7.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha8.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Alpha9.ToString()),
		});

		_keyboard[6].Dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
		{
			new TMP_Dropdown.OptionData(KeyCode.Space.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Q.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.Tab.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.E.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.R.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.F.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.LeftShift.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.LeftControl.ToString()),
			new TMP_Dropdown.OptionData(KeyCode.LeftAlt.ToString()),
		});

		//Keyboard keyboard = DataPersistenceManager.Instance.GameData.Keyboard;
		//if(keyboard.Keyboards.Count > 0)
		//{
		//	for(int i = 0; i < _keyboard.Count; i++)
		//	{
		//		_keyboard[i].Dropdown.value = 
		//			_keyboard[i].Dropdown.options.FindIndex(element => element.text == ((KeyCode)keyboard.Keyboards[_keyboard[i].Type]).ToString());
		//	}
		//}

	}

	private void OnEnable()
    {
        InitKeyboard();
		for (int i = 0; i < _keyboard.Count; i++)
		{
			KeyboardHandler(_keyboard[i].Dropdown, i);
		}

        _back.onClick.AddListener(Back);
		_save.onClick.AddListener(Save);
    }

	private void KeyboardHandler(TMP_Dropdown dropdown, int index)
	{
		dropdown.onValueChanged.AddListener((int indexSelected) => SetKeyboard(index));
	}

    private void SetKeyboard(int index)
	{
		Debug.Log($"{_keyboard[index].Type}: select {_keyboard[index].Dropdown.value} : {_keyboard[index].Dropdown.captionText.text}");

		//var keyboards = DataPersistenceManager.Instance.GameData.Keyboard;
		//keyboards.Keyboards[_keyboard[index].Type] = (int)Enum.Parse(typeof(KeyCode), _keyboard[index].Dropdown.captionText.text);
		//change and later save to local
	}

	private void Save()
	{
		//DataPersistenceManager.Instance.SaveData();
	}

    private void Back()
    {
        Option.Create();
        Destroy(gameObject);
    }

    private void OnDisable()
    {
		for (int i = 0; i < _keyboard.Count && i < 1; i++)
		{
			_keyboard[i].Dropdown.onValueChanged.RemoveListener(SetKeyboard);
		}

		_back.onClick.RemoveListener(Back);
		_save.onClick.RemoveListener(Save);
        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
