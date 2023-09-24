using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSimulation : MonoBehaviour
{
    public GameObject antPrefab;
    public int antCount;
    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < antCount; i++) {
            Instiante(antPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
