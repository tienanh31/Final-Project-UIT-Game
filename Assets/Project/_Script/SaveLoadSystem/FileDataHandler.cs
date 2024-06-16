using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    #region Fields & Properties

    private string dataDirPath = "";

    private string dataFileName = "";

    #endregion

    #region Methods 

    public FileDataHandler(string path, string fileName)
    {
        dataDirPath = path;
        dataFileName = fileName;
    }

    public void Load(ref GameData data)
    {
        string fullPath = System.IO.Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                string dataLoad = "";

                // readFile
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataLoad = reader.ReadToEnd();
                    }
                }
                Debug.Log(dataLoad);
                data = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(dataLoad);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

    }

    public void Save(GameData data)
    {
        string fullPath = System.IO.Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath));

            // format to JSON
            string dataStore = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Debug.Log(dataStore);
            // write file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    #endregion
}