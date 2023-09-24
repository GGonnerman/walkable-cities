using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class MainSimulation : MonoBehaviour
{
    public GameObject antPrefab;
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
    private GameObject ant;
    public int antCount;
    public TextAsset jsonFile;
    private List<GameObject> ants = new List<GameObject>();
    private int destinationIndex = 0;
    private bool antExists = true;
    PathingData pd;

    // Start is called before the first frame update
    void Start()
    {
        pd = JsonConvert.DeserializeObject<PathingData>(jsonFile.text);
        Debug.LogError(pd.path_data[0][1]);
        Debug.Log(pd.location_data);
        Debug.Log(pd.list_of_edges);

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
                    buildingType = capitalBuildingMaterial;
                    break;
                default:
                    buildingType = houseMaterial;
                    break;
            }

            building.GetComponent < MeshRenderer >().material = buildingType;
        }

        ant = Instantiate(antPrefab, new Vector3(pd.path_data[destinationIndex][0], 1, pd.path_data[destinationIndex][1]), Quaternion.identity);
        destinationIndex++;
        Debug.Log(ant);

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
        ant.GetComponent<Ant>().SetDestination(destination);
        ant.GetComponent<Ant>().SetSpeed(speed);
        /*for(int i = 0;  i < antCount; i++)
        {
            GameObject ant = Instantiate(antPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            ants.Add(ant);
            ant.GetComponent<Ant>().SetDestination(new Vector3(pd.path_data[destinationIndex][0], 0.5f, pd.path_data[destinationIndex][1]));

        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(ant);
        //Debug.Log(ant.GetComponent<Ant>());
        if(antExists)
        {

            if(ant.GetComponent<Ant>().IsAtDestination() )
            {
                destinationIndex += 1;
                if (destinationIndex >= pd.path_data.Length)
                {
                    Destroy(ant);
                    antExists = false;
                }
                else
                {
                    ant.GetComponent<Ant>().SetDestination(new Vector3(pd.path_data[destinationIndex][0], 0.5f, pd.path_data[destinationIndex][1]));
                }
            }

        }
            /*for (int j = 0; j < toHomePheromones.Length; j++)
            {
                if (toHomePheromones[i,j] != 0)
                {
                    Console.WriteLine("%f, %f: %f", i, j, toHomePheromones[i,j]);
                }
            }*/
    }
}
