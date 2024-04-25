using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : LevelManager
{
    public int condition = 0;
    [SerializeField] GameObject Jumppad, Door;

    //Drastic measures, should not be final
    protected override void Update()
    {
        base.Update();
        if (condition >= 2 && Jumppad != null && !Jumppad.gameObject.activeSelf)
            Jumppad.gameObject.SetActive(true);
        if (condition >= 4 && !Door.gameObject.activeSelf)
            Door.gameObject.SetActive(true);
    }

    public void DestroyJumppad()
    {
        Destroy(Jumppad.gameObject);
    }

    //Drastic measures, should not be final
    public void PlusCondition()
    {
        condition++;
    }
}
