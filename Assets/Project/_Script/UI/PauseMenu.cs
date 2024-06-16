using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu: MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] Button _mainMenu;
    [SerializeField] Button _option;
    [SerializeField] Button _saveGame;
    [SerializeField] Button _restartMap;

    public UI Type { get; set; }
    #endregion
    public static PauseMenu Create(Transform parent = null)
    {
        PauseMenu pause = Instantiate(Resources.Load<PauseMenu>("_Prefabs/UI/PauseMenu"), parent);
        pause.Type = UI.PAUSE;

        UIManager.Instance.UserInterfaces.Add(pause);
        return pause;
    }

    private void OnEnable()
    {
        _mainMenu.onClick.AddListener(BackToMainMenu);
        _option.onClick.AddListener(OpenOption);
        _saveGame.onClick.AddListener(SaveGame);
        _restartMap.onClick.AddListener(RestartMap);
    }

    private void BackToMainMenu()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene("Main Menu");

        MainMenu.Create();
    }

    private void OpenOption()
	{
        var option = Option.Create();
        option.PreviousUI = Type;

        Destroy(gameObject);
    }

    private void RestartMap()
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        UIManager.Instance.ResumeGame();
    }

    private void SaveGame()
	{
        Debug.Log("Save");
        GameManager.Instance.SaveData();
	}

    private void OnDisable()
    {
        _mainMenu.onClick.RemoveListener(BackToMainMenu);
        _option.onClick.RemoveListener(OpenOption);
        _saveGame.onClick.RemoveListener(SaveGame);
        _restartMap.onClick.RemoveListener(RestartMap);
        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
