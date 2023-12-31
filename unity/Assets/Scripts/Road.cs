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

        // Position the road directly between the two buildings it connects
        Vector2 midpoint = (start + end) / 2;
        transform.position = new Vector3(midpoint.x, 0, midpoint.y) ;

        // Use math to look at either of the buildings
        // end.y here is actually the z cord, but we read in a vector2 so there isnt a y
        float yRotation = 90f + (Mathf.Atan2(transform.position.x - end.x, transform.position.z - end.y) * 180 / Mathf.PI);
        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = new Vector3(0, yRotation, 90);
        transform.rotation = newRotation;

        // Symmetrically scale by half distance to reach both sides
        float distance = Vector2.Distance(start, end);
        transform.localScale += new Vector3(0, distance / 2, 0);
        // Increase texture tiling to look correct for our given distance
        GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, distance / 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
