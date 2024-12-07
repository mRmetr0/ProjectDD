using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float[] speedLevels;
    [SerializeField] private float fuel;
    [SerializeField] private float fuelUsage;
    [SerializeField] private int inventorySpace;
    public float[] SpeedLevels => speedLevels;
}
