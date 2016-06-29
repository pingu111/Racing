using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveCar : MonoBehaviour
{    
    /// <summary>
    /// the information about each individual wheel
    /// </summary>
    [SerializeField]
    public List<GameObject> listOfWheels;

    /// <summary>
    /// maximum torque the motor can apply to wheel
    /// </summary>

    public float maxMotorTorque;

    /// <summary>
    /// maximum steer angle the wheel can have
    /// </summary>
    public float maxSteeringAngle;

    /// <summary>
    /// the speed of this car
    /// </summary>
    public float carSpeed;

    private string nameLastWaypoint;

    /// <summary>
    /// The best line
    /// </summary>
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
        if (closestWayPoint != null && nameLastWaypoint != closestWayPoint.name)
        {
            // Make the car follow with teleportation

            /* Debug.Log("Closest : "+closestWayPoint.name);
             this.transform.position = closestWayPoint.position;
             Debug.Log("Position :"+this.transform.position);

             this.transform.LookAt(getClosestWayPointInFront());
             Debug.Log("Rotation to see the next one "+this.transform.localRotation);*/

            // We check if we have a good orientation
            //float angleCarWaypoint = Vector3.Angle(closestWayPoint.position - transform.position,this.transform.forward);
            //int sign = Vector3.Cross(closestWayPoint.position - transform.position, this.transform.forward).x < 0 ? -1 : 1;

            float angleCarWaypoint= AngleSigned(this.transform.forward, closestWayPoint.position - transform.position, Vector3.up);

            Debug.Log("TARGET : "+closestWayPoint.name);
            Debug.Log("angle : " + angleCarWaypoint);

            nameLastWaypoint = closestWayPoint.name;

            foreach (GameObject wheel in listOfWheels)
            {
                if (wheel.GetComponent<WheelsScript>().steering)
                {
                    wheel.GetComponent<WheelCollider>().steerAngle = angleCarWaypoint;
                }

            }
        }
    }

    /// <summary>
    /// Return  the closest waypoint, in the list bestLine, from the car.
    /// It can't be a waypoint in contact with the car
    /// </summary>

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

                // If we touch it, we go to the next one
                if (dist < minDist && 
                    !wayPoint.GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds))
                {
                    wayPointMin = wayPoint;
                    minDist = dist;
                }
            }
        }
        return wayPointMin;
    }

    /// <summary>
    /// Determine the signed angle between two vectors, with normal 'n'
    /// as the rotation axis.
    /// </summary>
    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return 
            Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)
                        ) * Mathf.Rad2Deg;
    }
}



