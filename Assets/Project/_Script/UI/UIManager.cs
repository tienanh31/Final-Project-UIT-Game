using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum UI
{
    MAIN_MENU = 0,
    PAUSE,
    WIN,
    LOSE,
    IN_GAME,
    OPTION,
    KEY,
    AUDIO,
    ENDING
}

public class UIManager : MonoBehaviour
{
    #region Fields & Properties
    public static UIManager Instance { get; private set; }

    public List<IUserInterface> UserInterfaces;

    public AudioMixerGroup _masterMixerGroup;
    public AudioMixerGroup _musicMixerGroup;
    public AudioMixerGroup _sfxMixerGroup;

    private AudioSource _mainMenuAudio;
    #endregion
    private void Awake()
	{
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(Instance);

        DataPersistenceManager.Instance.LoadData();

        UserInterfaces = new List<IUserInterface>();
        if (SceneManager.GetActiveScene().name == "Main Menu")
            MainMenu.Create();
    }

    public void ResumeGame()
    {
        RemoveAllUIInPlayGame();
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        PauseMenu.Create();
        Time.timeScale = 0;
    }
    
    public IUserInterface GetUI(UI type) => UserInterfaces.Find(element => element.Type == type);

    public void RemoveAllUIInPlayGame()
	{
        for (int i = UserInterfaces.Count - 1; i >= 0; i--)
		{
            if (UserInterfaces[i].Type == UI.IN_GAME)
			{
                return;
			}

            var ui = (UserInterfaces[i] as MonoBehaviour).transform;
            if (ui == null)
			{
                UserInterfaces.RemoveAt(i);
                return;
			}

            Destroy(ui.gameObject);
        }
    }

    public float GetMasterVolumn()
    {
        _masterMixerGroup.audioMixer.GetFloat("masterVolumn", out float a);
        return a;
    }

    public float GetMusicVolumn()
    {
        _musicMixerGroup.audioMixer.GetFloat("musicVolumn", out float a);
        return a;
    }

    public float GetSFXVolumn()
    {
        _sfxMixerGroup.audioMixer.GetFloat("sfxVolumn", out float a);
        return a;
    }

    public void SetMasterValue(float value)
    {
        _masterMixerGroup.audioMixer.SetFloat("masterVolumn", value);
    }

    public void SetMusicValue(float value)
    {
        _musicMixerGroup.audioMixer.SetFloat("musicVolumn", value);
    }

    public void SetSFXValue(float value)
    {
        _sfxMixerGroup.audioMixer.SetFloat("sfxVolumn", value);
    }
}
