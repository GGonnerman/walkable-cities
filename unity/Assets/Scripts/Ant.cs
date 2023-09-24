using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 GetLocation() {
        return this.transform.position;
    }

    void LookAt(Vector3 vector3) {
        float yRotation = 90f + (Mathf.Atan2(gameObject.transform.position.x, gameObject.transform.position.z) * 180 / Mathf.PI);
        Quaternion newRotation = Quaternion.identity; 
        newRotation.eulerAngles = new Vector3(0, yRotation, 90);
        gameObject.transform.rotation = newRotation;
    }

}
