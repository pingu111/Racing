using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveCar : MonoBehaviour
{
    // the information about each individual axle
    [SerializeField]
    public List<GameObject> listOfWheels;

    // maximum torque the motor can apply to wheel
    public float maxMotorTorque;

    // maximum steer angle the wheel can have
    public float maxSteeringAngle;

    // the speed of this car
    public float carSpeed;

    // The best line
    [SerializeField]
    public Transform bestLine;

    // Called at every physics steps
    public void FixedUpdate()
    {
        // If we touche the screen, we move !
        if (Input.touchCount > 0 || Input.GetAxis("Vertical") != 0)
        {
            carSpeed = maxMotorTorque;

            foreach (GameObject wheel in listOfWheels)
            {
                if (wheel.GetComponent<WheelsScript>().attachedToMotor)
                {
                    wheel.GetComponent<WheelCollider>().motorTorque = carSpeed;
                }

            }
        }
        else
            carSpeed = 0;

        // We are looking for the closest waypoint in BestLine
        Transform closestWayPoint = getClosestWayPointInFront();
        if (closestWayPoint != null)
        {
            Debug.Log(closestWayPoint.name);

            this.transform.position = closestWayPoint.position;
        }


    }

    Transform getClosestWayPoint()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Transform wayPoint in bestLine)
        {

            float dist = Vector3.Distance(wayPoint.position, currentPos);
            if (dist < minDist)
            {
                tMin = wayPoint;
                minDist = dist;
            }
        }

        return tMin;
    }

    Transform getClosestWayPointInFront()
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Transform wayPoint in bestLine)
        {
            float angleCarWaypoint = Vector3.Angle(this.transform.forward, wayPoint.position - transform.position);
            if (Mathf.Abs(angleCarWaypoint) < 90)
            {
                float dist = Vector3.Distance(wayPoint.position, currentPos);
                if (dist < minDist && dist != 0)
                {
                    tMin = wayPoint;
                    minDist = dist;
                }
            }
        }

        return tMin;
    }
}



