using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Slot
    {
        Primary,
        Secondary
    }

    public enum Type
    {
        Melee,
        Ranged,
        Support
    }

    public Slot slot;
    public Type type;
    public Skill skill;
}
