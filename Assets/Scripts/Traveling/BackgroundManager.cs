using System;
using System.Collections.Generic;
using Events;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;
using Slider = UnityEngine.UI.Slider;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;
    public static EventTile EventTile;
    
    [Header("Car")]
    [SerializeField] private Car car;
    [SerializeField] private Slider slider;
    [SerializeField] private float turnMod;
    [Header("Road")]
    [SerializeField] private RoadTile[] roadPrefabs;
    [SerializeField] private EventTile[] eventPrefabs;
    [SerializeField] private float tileDist;
    [SerializeField] List<RoadTile> roadtiles = new ();
    [Header("Traveling")] 
    [SerializeField] private int distToEvent;
    [SerializeField] [Range(1, 100)] private int eventOdds;

    private List<Transform> nodes = new ();
    private float carSpeed;
    private int tilesTraveled = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("MORE THEN ! BGMANAGER!?!?!?!?");
            return;
        }
        Instance = this;

        slider.maxValue = car.SpeedLevels.Length-1;
    }

    private void Update()
    {
        MoveRoad();
        MoveCar();
    }

    private void MoveCar()
    {
        if (nodes.Count == 0) return;
        Vector3 newPos = new Vector3(nodes[0].transform.position.x, 0, 0);
        car.transform.position = Vector3.Lerp(car.transform.position, newPos, carSpeed * Time.deltaTime * turnMod);
        if (nodes[0].position.z > car.transform.position.z) return;
        if (nodes[0].CompareTag("Event")) ShowEvent();
        nodes.RemoveAt(0);
        tilesTraveled++;
    }

    public void ReplaceCar(Car pCar)
    {
        Destroy(car.gameObject);
        car = Instantiate(pCar.gameObject, Vector3.zero, quaternion.identity).GetComponent<Car>();
        slider.maxValue = car.SpeedLevels.Length-1;
    }

    private void MoveRoad()
    {
        carSpeed = car.SpeedLevels[(int)Mathf.Ceil(slider.value)];
        Vector3 moveVec = new Vector3(0, 0, -1) * (carSpeed * Time.deltaTime);
        for (int i = roadtiles.Count - 1; i >= 0; i--)
        {
            RoadTile tile = roadtiles[i];
            tile.transform.position += moveVec;
            if (tile.transform.position.z > -1.5 * tileDist) continue;
            roadtiles.Remove(tile);
            Destroy(tile.gameObject);
            CreateTile(roadtiles[roadtiles.Count - 1].transform.position.z + tileDist);
        }
    }

    private void CreateTile(float distance)
    {
        Random rand = new Random();
        RoadTile prefab;
        RoadTile roadTile;
        if (tilesTraveled >= distToEvent && rand.Next(0, 101) <= eventOdds && EventTile == null)
        {
            prefab = eventPrefabs[rand.Next(0, eventPrefabs.Length)];
            roadTile = Instantiate(prefab, new Vector3(0, 0, distance), Quaternion.identity, transform);
            EventTile = roadTile as EventTile;
        } else {
            prefab = roadPrefabs[rand.Next(0, roadPrefabs.Length)];
            roadTile = Instantiate(prefab, new Vector3(0, 0, distance), Quaternion.identity, transform);
        }
        roadtiles.Add(roadTile);
        nodes.Add(roadTile.Node);
    }

    private void ShowEvent()
    {
        EventBus<OnShowEvent>.Publish(new OnShowEvent(EventTile));
        slider.value = 0;
        distToEvent = 0;
    }
}
