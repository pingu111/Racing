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
            Debug.Log("Closest : "+closestWayPoint.name);

            this.transform.position = closestWayPoint.position;
            Debug.Log("Position :"+this.transform.position);

            this.transform.LookAt(getClosestWayPointInFront());
            Debug.Log("Rotation to see the next one "+this.transform.localRotation);
        }


    }

    Transform getClosestWayPointInFront()
    {
        Transform wayPointMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (Transform wayPoint in bestLine)
        {
            // We verify if the waypoint is in front of the car 
            float angleCarWaypoint = Vector3.Angle(this.transform.forward, wayPoint.position - transform.position);
            if (Mathf.Abs(angleCarWaypoint) < 90)
            {
                float dist = Vector3.Distance(wayPoint.position, currentPos);

                // If we're too close, we go to the next one 
                if (dist < minDist && dist > 0.2f)
                {
                    wayPointMin = wayPoint;
                    minDist = dist;
                }
            }
        }

        Debug.Log("wayPointMin " + wayPointMin.name);
        Debug.Log("minDist " + minDist);

        return wayPointMin;
    }
}



