using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCharacter : CombatEntity
{
    private void Update()
    {
        TestMovement();
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    private void TestMovement()
    {
        int moveDir = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CombatManager.instance.MovePerson(this, moveDir);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CombatManager.instance.MovePerson(this, -moveDir);
        }
    }
}
