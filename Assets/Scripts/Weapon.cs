using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {
        Melee,
        Ranged,
        Support
    }

    public Type type;
    public Skill skill;
}
