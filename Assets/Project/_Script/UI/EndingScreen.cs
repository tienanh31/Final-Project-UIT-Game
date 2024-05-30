using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class EndingScreen : MonoBehaviour, IUserInterface
{
    #region Fields and Properties
    [SerializeField] Button _mainMenu;

    public UI Type { get; set; }
    public UI PreviousUI { get; set; }

    #endregion
    public static EndingScreen Create(Transform parent = null)
    {
        EndingScreen endingScreen = Instantiate(Resources.Load<EndingScreen>("_Prefabs/UI/Ending"), parent);
        endingScreen.Type = UI.ENDING;

        UIManager.Instance.UserInterfaces.Add(endingScreen);
        LevelManager.Instance.PauseGame();
        return endingScreen;
    }

    private void OnEnable()
    {
        _mainMenu.onClick.AddListener(BackToMainMenu);
    }


    private void BackToMainMenu()
    {
        Destroy(gameObject);
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene("Main Menu");

        MainMenu.Create();
        LevelManager.Instance.ResumeGame();
    }

    private void OnDisable()
    {
        _mainMenu.onClick.RemoveAllListeners();

        UIManager.Instance.UserInterfaces.Remove(this);
    }

}
