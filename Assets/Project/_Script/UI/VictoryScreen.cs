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
    [SerializeField] Button _hP;
    [SerializeField] Button _moveSpeed;
    [SerializeField] Button _attack;

    public UI Type { get; set; }
    public UI PreviousUI { get; set; }

    private int _buffType = 0;

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
        //if (nextLevel < SceneManager.sceneCountInBuildSettings &&  SceneManager.GetSceneByBuildIndex(nextLevel) != null)
        //{
        //    _nextLevel.gameObject.SetActive(true);
        //    Debug.Log($"Next level is {nextLevel}");
        //} else
        //{
        //    _nextLevel.gameObject.SetActive(false);
        //    Debug.Log($"Could not find the {nextLevel} scene! Scene Count: {SceneManager.sceneCount}");
        //}

        _mainMenu.onClick.AddListener(BackToMainMenu);
        _nextLevel.onClick.AddListener(NextLevel);
        _replayLevel.onClick.AddListener(RestartMap);

        _hP.onClick.AddListener(() => StatBonusHandler(0));
        _moveSpeed.onClick.AddListener(() => StatBonusHandler(1));
        _attack.onClick.AddListener(() => StatBonusHandler(2));

        _hP.onClick.Invoke();
    }

    private void StatBonusHandler(int type)
    {
        _buffType = type;

        var color = _hP.image.color;
        color.a = 0;
        _hP.image.color = color;

        color = _moveSpeed.image.color;
        color.a = 0;
        _moveSpeed.image.color = color;

        color = _attack.image.color;
        color.a = 0;
        _attack.image.color = color;

        switch (type)
        {
            case 0:
                color = _hP.image.color;
                color.a = 1;
                _hP.image.color = color;
                break;

            case 1:
                color = _moveSpeed.image.color;
                color.a = 1;
                _moveSpeed.image.color = color;
                break;

            case 2:
                color = _attack.image.color;
                color.a = 1;
                _attack.image.color = color;
                break;
        }
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
        //GameManager.Instance.LoadScene(nextLevel);

        GameManager.Instance.AddBuffStat(_buffType);
        GameManager.Instance.SaveData();
        GameManager.Instance.NextLevel();
        UIManager.Instance.ResumeGame();
    }

    private void OnDisable()
    {
        _mainMenu.onClick.RemoveAllListeners();
        _nextLevel.onClick.RemoveAllListeners();
        _replayLevel.onClick.RemoveAllListeners();

        _hP.onClick.RemoveListener(() => StatBonusHandler(0));
        _moveSpeed.onClick.RemoveListener(() => StatBonusHandler(1));
        _attack.onClick.RemoveListener(() => StatBonusHandler(2));

        UIManager.Instance.UserInterfaces.Remove(this);
    }

    public class BuffStat
    {
        public float HP = 8;
        public float MOVE_SPEED = 0.9f;
        public float ATTACK_BONUS = 7f;
    }    
}
