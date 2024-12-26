using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    [ReadOnly]
    public int position;
    
    
    //Publics:
    public int Position
    {
        get { return position; }
        set { position = value; }
    }
}
