using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BackgroundManager : MonoBehaviour
{
    [Header("Car")]
    [SerializeField] private GameObject car;
    [SerializeField] private float carSpeed;
    [SerializeField] private float turnMod;
    [Header("Road")]
    [SerializeField] private RoadTile[] roadPrefabs;
    [SerializeField] private float tileDist;
    [SerializeField] List<RoadTile> roadtiles = new ();
    
    private List<Transform> nodes = new ();

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
        if (nodes[0].position.z < car.transform.position.z) nodes.RemoveAt(0);
    }

    private void MoveRoad()
    {
        Vector3 MoveVec = new Vector3(0, 0, -1) * carSpeed * Time.deltaTime;
        for (int i = roadtiles.Count - 1; i >= 0; i--)
        {
            RoadTile tile = roadtiles[i];
            tile.transform.position += MoveVec;
            if (tile.transform.position.z > -tileDist) continue;
            roadtiles.Remove(tile);
            Destroy(tile.gameObject);
            CreateTile(roadtiles[roadtiles.Count - 1].transform.position.z + tileDist);
        }
    }

    private void CreateTile(float distance)
    {
        Random rand = new Random();
        RoadTile prefab = roadPrefabs[rand.Next(0, roadPrefabs.Length)];
        RoadTile roadTile = Instantiate(prefab, new Vector3(0, 0, distance), Quaternion.identity, transform);
        roadtiles.Add(roadTile);
        nodes.Add(roadTile.Node);
    }
}
