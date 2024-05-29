using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DefeatScreen : MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] Button _mainMenu;
    [SerializeField] Button _replayLevel;

    public UI Type { get; set; }
    public UI PreviousUI { get; set; }
    #endregion

    public static DefeatScreen Create(Transform parent = null)
    {
        DefeatScreen defeatScreen = Instantiate(Resources.Load<DefeatScreen>("_Prefabs/UI/Defeat"), parent);
        defeatScreen.Type = UI.LOSE;

        UIManager.Instance.UserInterfaces.Add(defeatScreen);
        return defeatScreen;
    }

    private void OnEnable()
    {
        _mainMenu.onClick.AddListener(BackToMainMenu);
        _replayLevel.onClick.AddListener(RestartMap);
    }

    private void BackToMainMenu()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene("Main Menu");

        MainMenu.Create();
        LevelManager.Instance.ResumeGame();
    }

    private void RestartMap()
    {
        GameManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
        UIManager.Instance.ResumeGame();
    }

    private void OnDisable()
    {
        _mainMenu.onClick.RemoveAllListeners();
        _replayLevel.onClick.RemoveAllListeners();

        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
