using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IUserInterface
{
    #region Description
    const string CHARACTER = "Default character, use for demo and debug only.";
    const string CHARACTER1 = "Character1, he has a powerful Shotgun and Grenade, can summon a drone.";
    const string CHARACTER2 = "Character2, he has a Resolver and Grenade, can summon a turret.";
    const string CHARACTER3 = "Character3, he has a Rifle and can lay Mine, can also summon a turret.";
    const string CHARACTER4 = "Character4, he has a Rifle and Cluster Grenade, and can laydown bullet proof walls.";
    #endregion

    #region Fields and Properties
    [SerializeField] TMP_Dropdown _characterSelecting;
    [SerializeField] TMP_Dropdown _levelSelecting;
    [SerializeField] Button _startGame;
    [SerializeField] Button _option;
    [SerializeField] Button _quit;
    [SerializeField] TMP_Text _description;
    [SerializeField] float _rotateSpeed = 0.5f;

    public UI Type { get; set; }

    private GameObject _model;
    private Vector3 _modelPlace;
    #endregion
    public static MainMenu Create(Transform parent = null)
	{
        MainMenu menu = Instantiate(Resources.Load<MainMenu>("_Prefabs/UI/MainMenu"), parent);
        menu.Type = UI.MAIN_MENU;

        UIManager.Instance.UserInterfaces.Add(menu);
        return menu;
    }

	private void OnEnable()
    {
        var place = GameObject.Find("ModelPlace");
        if(place)
        {
            _modelPlace = place.transform.position;
        }

        _characterSelecting.ClearOptions();
        _characterSelecting.AddOptions(new List<TMP_Dropdown.OptionData>()
        {
            new TMP_Dropdown.OptionData(typeof(Character).Name),
            new TMP_Dropdown.OptionData(typeof(Character1).Name),
            new TMP_Dropdown.OptionData(typeof(Character2).Name),
            new TMP_Dropdown.OptionData(typeof(Character3).Name),
            new TMP_Dropdown.OptionData(typeof(Character4).Name),
        });
        _characterSelecting.onValueChanged.AddListener(SetCharacter);
        _characterSelecting.value = 1;
        _characterSelecting.value = 0;

        _levelSelecting.ClearOptions();
        var levels = new List<TMP_Dropdown.OptionData>();
        Debug.Log(GameManager.Instance.TotalScene());
        for (int i = 1; i < GameManager.Instance.TotalScene(); i++)
		{
            levels.Add(new TMP_Dropdown.OptionData(i.ToString()));
		}
        _levelSelecting.AddOptions(levels);

        _startGame.onClick.AddListener(Begin);
        _option.onClick.AddListener(OpenOption);
        _quit.onClick.AddListener(Quit);
	}

	private void Update()
	{
		if(_model != null)
		{
            _model.transform.Rotate(0, _rotateSpeed, 0);
		}
	}

	private void SetCharacter(int indexSelecting)
    {
        GameManager.Instance.SelectedCharacter = (GameConfig.CHARACTER)indexSelecting;

        if (_model != null)
		{
            Destroy(_model.gameObject);
		}

        switch(indexSelecting)
		{
            case 0:
                _description.text = CHARACTER;
                _model = Instantiate(Resources.Load("_Prefabs/UI/Models/Character"), this.transform) as GameObject;
                break;

            case 1:
                _description.text = CHARACTER1;
                _model = Instantiate(Resources.Load("_Prefabs/UI/Models/Character 1"), this.transform) as GameObject;
                break;

            case 2:
                _description.text = CHARACTER2;
                _model = Instantiate(Resources.Load("_Prefabs/UI/Models/Character 2"), this.transform) as GameObject;
                break;

            case 3:
                _description.text = CHARACTER3;
                _model = Instantiate(Resources.Load("_Prefabs/UI/Models/Character 3"), this.transform) as GameObject;
                break;

            case 4:
                _description.text = CHARACTER4;
                _model = Instantiate(Resources.Load("_Prefabs/UI/Models/Character 4"), this.transform) as GameObject;
                break;
        }

        _model.transform.position = _modelPlace;
    }

    private void Begin()
    {
        GameManager.Instance.LoadScene(SceneName.PlayScene);
    }

    private void OpenOption()
    {
        var option = Option.Create();
        option.PreviousUI = Type;

        Destroy(gameObject);
    }

    private void Quit()
	{
        GameManager.Instance.QuitGame();
	}

	private void OnDisable()
	{
        _characterSelecting.onValueChanged.RemoveListener(SetCharacter);
        _startGame.onClick.RemoveListener(Begin);
        _option.onClick.RemoveListener(OpenOption);
        _quit.onClick.RemoveListener(Quit);
        UIManager.Instance.UserInterfaces.Remove(this);
	}
}
