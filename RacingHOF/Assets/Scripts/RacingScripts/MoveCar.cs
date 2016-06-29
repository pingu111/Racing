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


    /// <summary>
    /// Actual waypoint that the car is trying to get
    /// </summary>
    private Transform closestWayPoint;

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
                    wheel.GetComponent<WheelCollider>().brakeTorque = maxMotorTorque*10;
                }

            }
        }

        // We are looking for the closest waypoint in BestLine IF we passed the last one
        if (isWaypointPassed(idLastWaypoint))
        {
            closestWayPoint = getClosestWayPointInFront();

            // We calculate the steering needed for the wheels
            if (closestWayPoint != null)
            {
                float angleCarWaypoint = AngleSigned(this.transform.forward, closestWayPoint.position - transform.position, Vector3.up);

                Debug.Log("TARGET : " + closestWayPoint.name);

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
        int numberClosestWayPoint = -1;

        foreach (Transform wayPoint in bestLine)
        {
            // We verify if the waypoint is in front of the car 
            float angleCarWaypoint = Vector3.Angle(this.transform.forward, wayPoint.position - transform.position);
            if (Mathf.Abs(angleCarWaypoint) < 90)
            {
                float dist = Vector3.Distance(wayPoint.position, currentPos);

                // If we touch it, we go to the next one

                if (
                        dist < minDist // If it is closer than the best
                        &&  !wayPoint.GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) // If we don't touch it
                        && 
                        (
                            // If the ID is further than the last passed
                            ((numberActualWayPoint > idLastWaypoint ) 
                            && (Mathf.Abs(idLastWaypoint - numberActualWayPoint) < bestLine.childCount / 10) ) 
                            ||
                            // case for the end of the track : if we go from n°80 to n°1
                            ((numberActualWayPoint < idLastWaypoint ) 
                            && (numberActualWayPoint + idLastWaypoint > bestLine.childCount) 
                            && (idLastWaypoint > bestLine.childCount * 5/6) 
                            && (numberActualWayPoint < bestLine.childCount * 1/6))
                        )
                    ) 
                {
                    wayPointMin = wayPoint;
                    minDist = dist;
                    numberClosestWayPoint = numberActualWayPoint;
                }
            }
            numberActualWayPoint++;
        }

        if (numberClosestWayPoint == -1) // Then, we can't see any new waypoint : time to go to the last one
        {
            Debug.Log("Can't see anything, go to " + (idLastWaypoint +2));
            numberClosestWayPoint = idLastWaypoint + 2;
            if (numberClosestWayPoint >= bestLine.childCount)
                numberClosestWayPoint -= bestLine.childCount;
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
        if((Mathf.Abs(angleCarWaypoint) > 120 ||
                bestLine.GetChild(idLastWaypoint).GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds)))
            Debug.Log(idWayPoint + " passé ");

        return ( Mathf.Abs(angleCarWaypoint) > 120 || 
                bestLine.GetChild(idLastWaypoint).GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) );
    }
}



