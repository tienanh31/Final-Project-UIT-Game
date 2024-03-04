using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] Button _nextLevel;
    [SerializeField] Button _mainMenu;
    [SerializeField] Button _replayLevel;

    public UI Type { get; set; }
    public UI PreviousUI { get; set; }
    int nextLevel;
    #endregion

    public static VictoryScreen Create(Transform parent = null)
    {
        VictoryScreen victoryScreen = Instantiate(Resources.Load<VictoryScreen>("_Prefabs/UI/Victory"), parent);
        victoryScreen.Type = UI.WIN;

        UIManager.Instance.UserInterfaces.Add(victoryScreen);
        LevelManager.Instance.PauseGame();
        return victoryScreen;
    }

    private void OnEnable()
    {
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextLevel < SceneManager.sceneCountInBuildSettings &&  SceneManager.GetSceneByBuildIndex(nextLevel) != null)
        {
            _nextLevel.gameObject.SetActive(true);
            Debug.Log($"Next level is {nextLevel}");
        } else
        {
            _nextLevel.gameObject.SetActive(false);
            Debug.Log($"Could not find the {nextLevel} scene! Scene Count: {SceneManager.sceneCount}");
        }

        _mainMenu.onClick.AddListener(BackToMainMenu);
        _nextLevel.onClick.AddListener(NextLevel);
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

    private void NextLevel()
    {
        GameManager.Instance.LoadScene(nextLevel);
        UIManager.Instance.ResumeGame();
    }

    private void OnDisable()
    {
        _mainMenu.onClick.RemoveAllListeners();
        _nextLevel.onClick.RemoveAllListeners();
        _replayLevel.onClick.RemoveAllListeners();

        UIManager.Instance.UserInterfaces.Remove(this);
    }
}
