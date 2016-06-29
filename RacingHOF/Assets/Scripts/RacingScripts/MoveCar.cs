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
    /// The best line
    /// </summary>
    [SerializeField]
    public Transform bestLine;

    /// <summary>
    /// The number of the last waypoint that we must passed 
    /// </summary>
    private int idLastWaypoint;

    // Called at every physics steps
    public void FixedUpdate()
    {
        // If we touche the screen, we move !
         if (Input.touchCount > 0 || Input.GetAxis("Vertical") != 0)
         {
             // Update of the wheels
             foreach (GameObject wheel in listOfWheels)
             {
                 if (wheel.GetComponent<WheelsScript>().attachedToMotor)
                 {
                     wheel.GetComponent<WheelCollider>().brakeTorque = 0;
                     wheel.GetComponent<WheelCollider>().motorTorque = maxMotorTorque;
                 }
             }
         }
         else
         {
             // Update of the wheels
             foreach (GameObject wheel in listOfWheels)
             {
                 if (wheel.GetComponent<WheelsScript>().attachedToMotor)
                 {
                     wheel.GetComponent<WheelCollider>().motorTorque = 0;
                     wheel.GetComponent<WheelCollider>().brakeTorque = maxMotorTorque;
                 }

             }
         }
    
        // We are looking for the closest waypoint in BestLine IF we passed the last one
        if (isWaypointPassed(idLastWaypoint))
        {
            Transform closestWayPoint = getClosestWayPointInFront();
            if (closestWayPoint != null)
            {
                // We calculate the steering needed for the wheels
                float angleCarWaypoint = AngleSigned(this.transform.forward, closestWayPoint.position - transform.position, Vector3.up);

                Debug.Log("TARGET : " + closestWayPoint.name);
                Debug.Log("angle : " + angleCarWaypoint);

                foreach (GameObject wheel in listOfWheels)
                {
                    if (wheel.GetComponent<WheelsScript>().steering)
                    {
                        wheel.GetComponent<WheelCollider>().steerAngle = angleCarWaypoint;
                    }

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

        int numberActualWayPoint = 0;
        int numberClosestWayPoint = 0;

        foreach (Transform wayPoint in bestLine)
        {
            // We verify if the waypoint is in front of the car 
            float angleCarWaypoint = Vector3.Angle(this.transform.forward, wayPoint.position - transform.position);
            if (Mathf.Abs(angleCarWaypoint) < 90)
            {
                float dist = Vector3.Distance(wayPoint.position, currentPos);

                // If we touch it, we go to the next one
                if (
                        dist < minDist && // If it is closer than the best
                        !wayPoint.GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) && // If we don't touch it
                        ( numberActualWayPoint > idLastWaypoint ) ||// If the ID is further than the last passed
                        (( numberActualWayPoint < idLastWaypoint ) 
                        && (numberActualWayPoint + idLastWaypoint > bestLine.childCount) 
                        && (numberActualWayPoint > bestLine.childCount/2)
                        && (idLastWaypoint < bestLine.childCount / 2)) // case for the end of the track
                    ) 
                {
                    wayPointMin = wayPoint;
                    minDist = dist;
                    numberClosestWayPoint = numberActualWayPoint;
                }
            }
            numberActualWayPoint++;
        }

        Debug.Log("idLastWaypoint " + idLastWaypoint + " / numberClosestWayPoint " + numberClosestWayPoint);
        idLastWaypoint = numberClosestWayPoint;
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

    /// <summary>
    /// Return if a waypoint is passed : 
    /// true if behind the car OR if touched
    /// </summary>
    /// <param name="idWayPoint">The id of the waypoint to check</param>
    /// <returns>true if behind the car OR if touched</returns>
    bool isWaypointPassed(int idWayPoint)
    {
        float angleCarWaypoint = Vector3.Angle(this.transform.forward, bestLine.GetChild(idLastWaypoint).position - transform.position);

        return ( Mathf.Abs(angleCarWaypoint) > 90 || 
                bestLine.GetChild(idLastWaypoint).GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) );
    }
}



