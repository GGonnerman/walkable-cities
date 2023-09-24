using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Ant : MonoBehaviour
{

    private float speed = 10f;
    private float tolerance = 1f;
    private Vector3 destination;
    // Start is called before the first frame update
    void Start()
    {
        // Spawn in a random location
        //Vector3 location = new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 0, UnityEngine.Random.Range(-10.0f, 10.0f));
        //Vector3 location = new Vector3(50, 0, 50);

        // Be flat on the floor
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0, 0, 90);
        //rotation.eulerAngles = new Vector3(0, UnityEngine.Random.Range(-90f, 90f), 90);

        // Set to use location and rotation
        //transform.position = location;
        transform.rotation = rotation;

        LookAt(destination);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.position += transform.forward * Time.deltaTime * movementSpeed;
        MoveFoward();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetTolerance(float tolerance)
    {
        this.tolerance = tolerance;
    }

    private void MoveFoward()
    {

        float angle = transform.rotation.eulerAngles.y * Mathf.PI / 180;
        transform.position += new Vector3(
            Mathf.Cos(angle) * speed * Time.deltaTime,
            0,
            -Mathf.Sin(angle) * speed * Time.deltaTime
        );
        // TODO: Check for overflow
        // x = sin(angle) * speed * deltaTime
        // z = cos(angle) * speed * deltaTime

    }

    public void LookAt(Vector3 lookLocation)
    {
        Debug.Log("Running Look at function");
        float yRotation = 90f + (Mathf.Atan2(gameObject.transform.position.x - lookLocation.x, gameObject.transform.position.z - lookLocation.z) * 180 / Mathf.PI);
        Debug.Log("Rotation is " + yRotation);
        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = new Vector3(0, yRotation, 90);
        gameObject.transform.rotation = newRotation;
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        Debug.Log("Set destionation to " + destination);
        LookAt(destination);
    }

    public bool IsAtDestination()
    {
        if ((transform.position - destination).magnitude < tolerance)
        {
            return true;
        }
        return false;
    }
}
