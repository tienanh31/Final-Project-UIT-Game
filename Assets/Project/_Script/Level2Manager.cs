using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Manager : LevelManager
{
    override public bool WinCondition()
    {
        return false;
    }

    protected override void Start()
    {
        base.Start();
    }
}
