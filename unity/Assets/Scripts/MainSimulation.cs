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
        ant = Instantiate(antPrefab, new Vector3(50, 0, 50), Quaternion.identity);
        Debug.Log(ant);

        pd = JsonConvert.DeserializeObject<PathingData>(jsonFile.text);
        Debug.LogError(pd.path_data[0][1]);
        Debug.Log(pd.location_data);

        foreach (var item in pd.location_data)
        {
            string name = item.Key;
            Debug.Log("We have " + name);
            int[] location = item.Value;
            Debug.Log(location);
            Debug.Log(location[0]);
            Instantiate(buildingPrefab, new Vector3(location[0], 0.5f, location[1]), Quaternion.identity);
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
