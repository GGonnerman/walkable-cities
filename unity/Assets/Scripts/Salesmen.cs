using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;

public class Salesmen : MonoBehaviour
{

    private float speed = 20f;
    private float augmentedSpeed = 20f;
    private float tolerance = 1f;
    private Vector3 destination;

    void Start()
    {
        // Be sideways on the floor
        Quaternion rotation = Quaternion.identity;
        rotation.eulerAngles = new Vector3(0, 0, 90);
        transform.rotation = rotation;
        LookAt(destination);
    }

    void FixedUpdate()
    {
        MoveFoward();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    // Update tolerance to destination before having "arrived"
    public void SetTolerance(float tolerance)
    {
        this.tolerance = tolerance;
    }

    // Move forward with our current angle scaling for change in time and augmentedSpeed (speed with elevation gradient factored in) 
    private void MoveFoward()
    {
        float angle = transform.rotation.eulerAngles.y * Mathf.PI / 180;
        transform.position += new Vector3(
            Mathf.Cos(angle) * augmentedSpeed * Time.deltaTime,
            0,
            -Mathf.Sin(angle) * augmentedSpeed * Time.deltaTime
        );
    }

    // Use math to look at an abstract vector in 3d space
    public void LookAt(Vector3 lookLocation)
    {
        float yRotation = 90f + (Mathf.Atan2(gameObject.transform.position.x - lookLocation.x, gameObject.transform.position.z - lookLocation.z) * 180 / Mathf.PI);
        Quaternion newRotation = Quaternion.identity;
        newRotation.eulerAngles = new Vector3(0, yRotation, 90);
        gameObject.transform.rotation = newRotation;
    }

    // Add the current destination (the place to look)
    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        LookAt(destination);
    }

    // Update augmented speed based on the difficulty (or ease) of the elevation change
    public void SetDifficulty(float difficulty)
    {
        augmentedSpeed = speed + 0.5f * difficulty * speed;
    }

    // Check if within a tolerance of overall destination
    public bool IsAtDestination()
    {
        if ((transform.position - destination).magnitude < tolerance)
        {
            return true;
        }
        return false;
    }
}
