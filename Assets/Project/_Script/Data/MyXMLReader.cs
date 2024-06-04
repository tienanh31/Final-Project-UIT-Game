using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class MyXMLReader
{
    private string _fileName = "data.xml";

    private XmlDocument _data;

    public void ReadFile()
    {
        TextAsset xmlData = new TextAsset();
        xmlData = Resources.Load<TextAsset>("Data\\data");

        _data = new XmlDocument();
        _data.LoadXml(xmlData.text);

        var items = _data.SelectNodes("/Data/Level");
        Debug.Log(items[0].ChildNodes.Count);
    }

    public Dictionary<GameConfig.ENEMY, int> GetEnemiesAtLevel(int level)
    {
        Dictionary<GameConfig.ENEMY, int> enemies = new Dictionary<GameConfig.ENEMY, int>();

        var levelData = FindLevelWithId(level);
        
        if (levelData != null)
        {
            int count = 0;
            foreach(XmlNode enemy in levelData.ChildNodes)
            {
                int value = 0;
                int.TryParse(enemy.InnerText, out value);

                enemies.Add((GameConfig.ENEMY)count++, value);
            }
        }
        
        return enemies;
    }

    private XmlNode FindLevelWithId(int id)
    {
        var node = _data.SelectSingleNode("Data/Level[@Id='" + id + "']");

        return node;
    }

}
