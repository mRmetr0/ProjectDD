using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float[] speedLevels;
    [SerializeField] private float fuel;
    [SerializeField] private float fuleUsage;

    public float[] SpeedLevels => speedLevels;

}
