using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.Rendering.CameraUI;

public class MainSimulation : MonoBehaviour
{
    public City chosenCity;
    public GameObject salesmenPrefab;
    public GameObject buildingPrefab;
    public GameObject roadPrefab;
    public Material trainMaterial;
    public Material houseMaterial;
    public Material hospitalMaterial;
    public Material policeStationMaterial;
    public Material fireStationMaterial;
    public Material shopMaterial;
    public Material capitalBuildingMaterial;
    public float speed;
    public float tolerance;
    private GameObject salesmen;
    public int salesmenCount;
    public TextAsset iowaPrims;
    public TextAsset iowaElevation;
    public Material iowaMaterial;
    public TextAsset bostonPrims;
    public TextAsset bostonElevation;
    public Material bostonMaterial;
    public TextAsset detroitPrims;
    public TextAsset detroitElevation;
    public Material detroidMaterial;
    private List<GameObject> samesmens = new List<GameObject>();
    private int destinationIndex = 0;
    private bool salesmenExists = true;
    HeightData height_json;
    private int[][] height_data;
    private int highestPoint = -1;
    private int lowestPoint = 20000;
    private int elevationDelta;
    PathingData pd;

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

        if(chosenCity == City.Iowa)
        {
            primsFile = iowaPrims;
            elevationFile = iowaElevation;
            groundMaterial = iowaMaterial;

        }
        else if(chosenCity == City.Detroit)
        {
            primsFile = detroitPrims;
            elevationFile = detroitElevation;
            groundMaterial = detroidMaterial;
        }
        else if(chosenCity == City.Boston)
        {
            primsFile = bostonPrims;
            elevationFile = bostonElevation;
            groundMaterial = bostonMaterial;
        }
        else
        {
            Debug.LogError("No Valid City Chosen");
        }

        pd = JsonConvert.DeserializeObject<PathingData>(primsFile.text);
        height_json = JsonConvert.DeserializeObject<HeightData>(elevationFile.text);
        GameObject ground = GameObject.Find("Plane");
        ground.GetComponent<MeshRenderer>().material = groundMaterial;

        for (int i = 0; i < height_json.data.Length; i++)
        {
            for(int j = 0; j < height_json.data[i].Length; j++)
            {
                if (height_json.data[i][j] > highestPoint) {  highestPoint = height_json.data[i][j]; }
                if (height_json.data[i][j] < lowestPoint) {  lowestPoint = height_json.data[i][j]; }
            }
        }
        elevationDelta = highestPoint - lowestPoint;

        foreach (var item in pd.location_data)
        {
            string name = item.Key;
            Debug.Log("We have " + name);
            int[] location = item.Value;
            Debug.Log(location);
            Debug.Log(location[0]);
            GameObject building = Instantiate(buildingPrefab, new Vector3(location[0], 2f, location[1]), Quaternion.identity);
            Material buildingType;

            switch(name)
            {
                //case "train":
                //    buildingType = trainMaterial;
                case "house":
                    buildingType = houseMaterial;
                    break;
                case "hospital":
                    buildingType = hospitalMaterial;
                    break;
                case "police_station":
                    buildingType = policeStationMaterial;
                    break;
                //case "fire_station":
                //    break;
                //case "shop":
                //    break;
                case "capital_building":
                    if(chosenCity == City.Iowa) {
                        buildingType = capitalBuildingMaterial;
                    } else
                    {
                        buildingType = houseMaterial;
                    }
                    break;
                default:
                    buildingType = houseMaterial;
                    break;
            }

            building.GetComponent < MeshRenderer >().material = buildingType;
        }

        salesmen = Instantiate(salesmenPrefab, new Vector3(pd.path_data[destinationIndex][0], 1, pd.path_data[destinationIndex][1]), Quaternion.identity);
        salesmen.GetComponent<Salesmen>().SetSpeed(speed);
        destinationIndex++;

        salesmen.GetComponent<Salesmen>().SetDifficulty (
            GetDifficulty(
                height_json.data[ pd.path_data[destinationIndex - 1][1] ][ pd.path_data[destinationIndex - 1][1] ],
                height_json.data[ pd.path_data[destinationIndex][1] ][ pd.path_data[destinationIndex][1] ]
            )
        );

        Debug.Log(salesmen);

        Debug.Log("About to print edges");
        Debug.Log(pd.list_of_edges);
        foreach (var edge in pd.list_of_edges)
        {
            //Debug.Log(edge);
            //Debug.Log(edge[0]);
            //Debug.Log(edge[0][0]);
            Vector2 start = new Vector2(edge[0][0], edge[0][1]);
            Vector2 end = new Vector2(edge[1][0], edge[1][1]);

            GameObject road = Instantiate(roadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            road.GetComponent<Road>().SetEndpoints(start, end);

        }

        Vector3 destination = new Vector3(pd.path_data[destinationIndex][0], 0.5f, pd.path_data[destinationIndex][1]);
        Debug.Log(destination);
        salesmen.GetComponent<Salesmen>().SetDestination(destination);
        /*for(int i = 0;  i < antCount; i++)
        {
            GameObject salesmen = Instantiate(salesmenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            salesmens.Add(ant);
            salesmen.GetComponent<Salesmen>().SetDestination(new Vector3(pd.path_data[destinationIndex][0], 0.5f, pd.path_data[destinationIndex][1]));

        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(salesmen);
        //Debug.Log(salesmen.GetComponent<Salesmen>());
        if(salesmenExists)
        {

            if(salesmen.GetComponent<Salesmen>().IsAtDestination() )
            {
                destinationIndex += 1;
                if (destinationIndex >= pd.path_data.Length)
                {
                    Destroy(salesmen);
                    salesmenExists = false;
                }
                else
                {
                    salesmen.GetComponent<Salesmen>().SetDestination(new Vector3(pd.path_data[destinationIndex][0], 0.5f, pd.path_data[destinationIndex][1]));

                    salesmen.GetComponent<Salesmen>().SetDifficulty (
                        GetDifficulty(
                            height_json.data[ pd.path_data[destinationIndex - 1][1] ][ pd.path_data[destinationIndex - 1][0] ],
                            height_json.data[ pd.path_data[destinationIndex][1] ][ pd.path_data[destinationIndex][0] ]
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
