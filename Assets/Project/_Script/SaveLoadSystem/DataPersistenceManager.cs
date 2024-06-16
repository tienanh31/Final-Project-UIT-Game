using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager
{
    #region Fields & Properties

    private static DataPersistenceManager instance;
    public static DataPersistenceManager Instance
    {
        get { if (instance == null) instance = new DataPersistenceManager(); return instance; }
        private set { instance = value; }
    }

    [Header("File Storage Config")]

    [SerializeField] private string _fileName = "data.Game";

    private FileDataHandler _dataHandler;

    public GameData GameData;

    #endregion

    #region Methods

    private DataPersistenceManager()
    {
        this._dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);

        Debug.Log(Application.persistentDataPath);
        LoadData();
        if (GameData == null)
		{
            NewData();
		}
    }

    public void NewData()
    {
        this.GameData = new GameData();
    }

    public void LoadData()
    {
        _dataHandler.Load(ref GameData);
    }

    public void SaveData()
    {
        GameData.ClearData();

        _dataHandler.Save(GameData);
    }

    #endregion
}