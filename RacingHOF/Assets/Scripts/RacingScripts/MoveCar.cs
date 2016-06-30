using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script that move the Player car
/// </summary>
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
    public float maxMotorTorque = 200;

    /// <summary>
    /// maximum steer angle the wheel can have
    /// </summary>
    public float maxSteeringAngle = 30;

    /// <summary>
    /// The best line
    /// </summary>
    [SerializeField]
    public Transform bestLine;

    /// <summary>
    /// The number of the last waypoint that we must passed 
    /// </summary>
    private int idLastWaypoint = 0;

    /// <summary>
    /// Actual waypoint that the car is trying to get
    /// </summary>
    private Transform closestWayPoint;

    /// <summary>
    /// Number of waypoint that we pass if we can't see anything
    /// </summary>
    private int advanceIfCantSee = 2;

    /// <summary>
    /// Called at every physics steps
    /// </summary>
    public void FixedUpdate()
    {
        /// The motor power of the car, applied to the wheels
        float motorTorque = 0;

        // If we touche the screen, we move 
        if (Input.touches.Length != 0 || Input.GetAxis("Vertical") != 0)
        {
            motorTorque = maxMotorTorque;
        }
        else // Then we brake
            this.GetComponent<Rigidbody>().velocity *= 0.98f;

        // Update of the motor on the wheels and the brake attached to the motor
        foreach (GameObject wheel in listOfWheels)
        {
            if (wheel.GetComponent<WheelsScript>().attachedToMotor)
            {
                wheel.GetComponent<WheelCollider>().motorTorque = motorTorque;
            }
        }

        // We are looking for the closest waypoint in BestLine IF we passed the last one
        if (isWaypointPassed(idLastWaypoint))
        {
            closestWayPoint = getClosestWayPointInFront();
        }

        // Update of the steerAngle of the wheels
        if (closestWayPoint != null)
        {
            // Calculate of the best angle between the front of the car and the waypoint
            float angleCarWaypoint = AngleSigned(this.transform.forward, closestWayPoint.position - transform.position, Vector3.up);

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
    /// It can't be a waypoint in contact with the car of behind the car
    /// </summary>
    /// <returns>Transform wayPointMin the closest waypoint in front of the car</returns>
    Transform getClosestWayPointInFront()
    {
        /// The closest waypoint
        Transform wayPointMin = null;

        /// The minimal distance found to the closest waypoint
        float minDist = Mathf.Infinity;
        
        /// The id of  the waypoint that we are currently seeing
        int numberActualWayPoint = 0;

        /// The id of the closest waypoint found
        int numberClosestWayPoint = -1;

        foreach (Transform wayPoint in bestLine)
        {
            // We verify if the waypoint is in front of the car 
            float angleCarWaypoint = Vector3.Angle(this.transform.forward, wayPoint.position - transform.position);
            if (Mathf.Abs(angleCarWaypoint) < 90)
            {
                float distanceWaypointCar = Vector3.Distance(wayPoint.position, transform.position);

                // Condition for a waypoint to be the next and closest
                if (
                        distanceWaypointCar < minDist // If it is closer
                        &&  !wayPoint.GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) // If we don't touch it
                        && 
                        (
                            ((numberActualWayPoint > idLastWaypoint)  // If the ID is further than the last passed
                            && (Mathf.Abs(idLastWaypoint - numberActualWayPoint) < bestLine.childCount / 10) ) // If it's not too far away
                            ||
                            // case for the end of the track : if we go from n°80 to n°1 by exemple
                            ((numberActualWayPoint < idLastWaypoint ) 
                            && (numberActualWayPoint + idLastWaypoint > bestLine.childCount) 
                            && (idLastWaypoint > bestLine.childCount * 5/6) 
                            && (numberActualWayPoint < bestLine.childCount * 1/6))
                        )
                    ) 
                {
                    // Then it's a good waypoint 
                    wayPointMin = wayPoint;
                    minDist = distanceWaypointCar;
                    numberClosestWayPoint = numberActualWayPoint;
                }
            }
            numberActualWayPoint++;
        }

        if (numberClosestWayPoint == -1) // Then, we can't see any new waypoint : time to go to the last known one plus X
        {
            numberClosestWayPoint = idLastWaypoint + advanceIfCantSee;
            if (numberClosestWayPoint >= bestLine.childCount)
                numberClosestWayPoint -= bestLine.childCount;
        }

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
        return ( Mathf.Abs(angleCarWaypoint) > 60 || 
                bestLine.GetChild(idLastWaypoint).GetComponent<Renderer>().bounds.Intersects(this.GetComponent<Collider>().bounds) );
    }
}



