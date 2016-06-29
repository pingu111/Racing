using UnityEngine;
using System.Collections;

/*
 * Script that made the Camera following the GameObject 
 */
public class CameraFollow : MonoBehaviour {

    // The gameObject that we want to follow 
    [SerializeField]
    GameObject objectToFollow;

	// Use this for initialization
	void Start ()
    {
	
	}

    // Update is called once per frame
    void Update ()
    {
        // We follow the gameObject 

        Vector3 velocity = Vector3.zero;
        Vector3 needPos = objectToFollow.transform.position;


        transform.position = Vector3.SmoothDamp(transform.position, needPos, ref velocity, 0f);
        transform.rotation = objectToFollow.transform.rotation;

        transform.LookAt(objectToFollow.transform);

    }
}

