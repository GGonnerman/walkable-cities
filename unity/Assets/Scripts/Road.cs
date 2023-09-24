using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetEndpoints(Vector2 start, Vector2 end)
    {
        if (start == end) return;
        Vector2 midpoint = (start + end) / 2;
        transform.position = new Vector3(midpoint.x, 0, midpoint.y) ;

        Debug.Log("Running Look at function");
        // end.y here is actually the z cord, but we read in a vector2 so there isnt a y
        float yRotation = 90f + (Mathf.Atan2(transform.position.x - end.x, transform.position.z - end.y) * 180 / Mathf.PI);
        Debug.Log("Rotation is " + yRotation);
        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = new Vector3(0, yRotation, 90);
        transform.rotation = newRotation;
        Debug.Log("About to scale");
        transform.localScale += new Vector3(0, Vector2.Distance(start, end) / 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
