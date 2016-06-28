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
    public GameObject bestLine;

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

        Vector3 pos = this.transform.position;
        pos.z = transform.position.z - Camera.main.transform.position.z;
        pos = Camera.main.ScreenToWorldPoint(pos);

        Vector3 placement = findClosest(bestLine.GetComponent<PolygonCollider2D>(), pos);
        placement.z = transform.position.z;
        Debug.Log( placement);
    }

    private Vector2 findClosest(PolygonCollider2D collider, Vector2 positionCar)
    {

        float closestDistance = Mathf.Infinity;
        Vector2 closestPoint = Vector2.zero;

        foreach (Vector2 colliderPoint in collider.points)
        {
            Vector2 worldSpacePoint = collider.transform.TransformPoint(colliderPoint);
            float distance = Vector2.Distance(worldSpacePoint, positionCar);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = worldSpacePoint;
            }
        }
        Debug.Log("closestDistance="+closestDistance);

        return closestPoint;
    }
}



