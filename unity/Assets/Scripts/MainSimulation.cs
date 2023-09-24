using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Rendering.CameraUI;

public class MainSimulation : MonoBehaviour
{
    public City chosenCity;
    public bool isLayered;
    public GameObject salesmenPrefab;
    public GameObject buildingPrefab;
    public GameObject roadPrefab;
    public Material trainMaterial;
    public Material houseMaterial;
    public Material hospitalMaterial;
    public Material policeStationMaterial;
    public Material fireStationMaterial;
    public Material shopMaterial;
    public Material apartmentMaterial;
    public Material capitalBuildingMaterial;
    public Material gardnerMuseumMaterial;
    public float speed;
    public float tolerance;
    private GameObject salesmen;
    public int salesmenCount;
    public TextAsset iowaPrims;
    public TextAsset iowaElevation;
    public Material iowaMaterial;
    public Material iowaLayeredMaterial;
    public TextAsset bostonPrims;
    public TextAsset bostonElevation;
    public Material bostonMaterial;
    public Material bostonLayeredMaterial;
    public TextAsset detroitPrims;
    public TextAsset detroitElevation;
    public Material detroitMaterial;
    public Material detroitLayeredMaterial;
    private List<GameObject> samesmens = new List<GameObject>();
    private int destinationIndex = 0;
    private bool salesmenExists = true;
    HeightData heightJson;
    private int[][] height_data;
    private int highestPoint = -1;
    private int lowestPoint = 20000;
    private int elevationDelta;
    PathingData pathingJson;

    public enum City {
        Iowa,
        Detroit,
        Boston
    }

    // Start is called before the first frame update
    void Start()
    {
        TextAsset primsFile = null;
        TextAsset elevationFile = null;
        Material groundMaterial = null;

        // Set the prims pathfinding file, elevation file, and ground material based on the user-chosen city and layer style
        if(chosenCity == City.Iowa)
        {
            primsFile = iowaPrims;
            elevationFile = iowaElevation;
            groundMaterial = isLayered ? iowaLayeredMaterial : iowaMaterial;
        }
        else if(chosenCity == City.Detroit)
        {
            primsFile = detroitPrims;
            elevationFile = detroitElevation;
            groundMaterial = isLayered ? detroitLayeredMaterial : detroitMaterial;
        }
        else if(chosenCity == City.Boston)
        {
            primsFile = bostonPrims;
            elevationFile = bostonElevation;
            groundMaterial = isLayered ? bostonLayeredMaterial : bostonMaterial;
        }
        else
        {
            Debug.LogError("No Valid City Chosen");
        }

        GameObject ground = GameObject.Find("Plane");
        ground.GetComponent<MeshRenderer>().material = groundMaterial;
        pathingJson = JsonConvert.DeserializeObject<PathingData>(primsFile.text);
        heightJson = JsonConvert.DeserializeObject<HeightData>(elevationFile.text);

        // Get the high and low bounds of elevation for normalizing the gradient
        for (int i = 0; i < heightJson.data.Length; i++)
        {
            for(int j = 0; j < heightJson.data[i].Length; j++)
            {
                if (heightJson.data[i][j] > highestPoint) {  highestPoint = heightJson.data[i][j]; }
                if (heightJson.data[i][j] < lowestPoint) {  lowestPoint = heightJson.data[i][j]; }
            }
        }
        elevationDelta = highestPoint - lowestPoint;

        // Create a new building for each item in location data
        foreach (var item in pathingJson.location_data)
        {
            string name = item.Key;
            int[] location = item.Value;
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, 180, 0);
            GameObject building = Instantiate(buildingPrefab, new Vector3(location[0], 2f, location[1]), rotation);
            Material buildingType;

            // Set building material based on the building type
            switch(name)
            {
                case "house":
                    buildingType = houseMaterial;
                    break;
                case "hospital":
                    buildingType = hospitalMaterial;
                    break;
                case "police_station":
                    buildingType = policeStationMaterial;
                    break;
                case "fire_station":
                    buildingType = fireStationMaterial;
                    break;
                case "train":
                    buildingType = trainMaterial;
                    break;
                case "shop":
                    buildingType = shopMaterial;
                    break;
                case "apartment":
                    buildingType = apartmentMaterial;
                    break;
                case "capital_building":
                    // Set the "special building" for each city
                    if (chosenCity == City.Iowa)
                    {
                        buildingType = capitalBuildingMaterial;
                    }
                    else if (chosenCity == City.Boston)
                    {
                        buildingType = gardnerMuseumMaterial;
                    }
                    else
                    {
                        buildingType = houseMaterial;
                    }
                    break;
                default:
                    // Random distribution of "common" buildings
                    float randomValue = UnityEngine.Random.Range(0, 100f);
                    if(randomValue < 20)
                    {
                        buildingType = shopMaterial;
                    }
                    else if (randomValue < 60)
                    {
                        buildingType = houseMaterial;
                    }
                    else
                    {
                        buildingType = apartmentMaterial;
                    }
                    break;
            }

            building.GetComponent < MeshRenderer >().material = buildingType;
        }

        // Create the salesmen at the first location, with a new destination and difficulty
        salesmen = Instantiate(salesmenPrefab, new Vector3(pathingJson.path_data[destinationIndex][0], 1, pathingJson.path_data[destinationIndex][1]), Quaternion.identity);
        salesmen.GetComponent<Salesmen>().SetSpeed(speed);
        destinationIndex++;

        salesmen.GetComponent<Salesmen>().SetDifficulty (
            GetDifficulty(
                heightJson.data[ pathingJson.path_data[destinationIndex - 1][1] ][ pathingJson.path_data[destinationIndex - 1][1] ],
                heightJson.data[ pathingJson.path_data[destinationIndex][1] ][ pathingJson.path_data[destinationIndex][1] ]
            )
        );

        // Draw each edge between 2 vertexes (houses) as a road
        foreach (var edge in pathingJson.list_of_edges)
        {
            Vector2 start = new Vector2(edge[0][0], edge[0][1]);
            Vector2 end = new Vector2(edge[1][0], edge[1][1]);

            GameObject road = Instantiate(roadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            road.GetComponent<Road>().SetEndpoints(start, end);

        }

        Vector3 destination = new Vector3(pathingJson.path_data[destinationIndex][0], 0.5f, pathingJson.path_data[destinationIndex][1]);
        Debug.Log(destination);
        salesmen.GetComponent<Salesmen>().SetDestination(destination);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure salesmen hasn't completed his path
        if(salesmenExists)
        {

            if(salesmen.GetComponent<Salesmen>().IsAtDestination() )
            {
                // If at destination,, update it and get a new difficult (elevation diff) for his new path
                destinationIndex += 1;
                if (destinationIndex >= pathingJson.path_data.Length)
                {
                    Destroy(salesmen);
                    salesmenExists = false;
                }
                else
                {
                    salesmen.GetComponent<Salesmen>().SetDestination(new Vector3(pathingJson.path_data[destinationIndex][0], 0.5f, pathingJson.path_data[destinationIndex][1]));

                    salesmen.GetComponent<Salesmen>().SetDifficulty (
                        GetDifficulty(
                            heightJson.data[ pathingJson.path_data[destinationIndex - 1][1] ][ pathingJson.path_data[destinationIndex - 1][0] ],
                            heightJson.data[ pathingJson.path_data[destinationIndex][1] ][ pathingJson.path_data[destinationIndex][0] ]
                        )
                    );
                }
            }

        }
    }

    float GetDifficulty(float start, float end)
    {
        Debug.Log(string.Format($"{start} start, {end} end"));
        float scaled_incline = ((end - start) / elevationDelta);
        return scaled_incline;
    }
}
