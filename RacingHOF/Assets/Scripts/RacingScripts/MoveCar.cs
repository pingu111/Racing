using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveCar : MonoBehaviour
{
    // the information about each individual axle
    public List<GameObject> listOfWheels;

    // maximum torque the motor can apply to wheel
    public float maxMotorTorque;

    // maximum steer angle the wheel can have
    public float maxSteeringAngle;

    // the speed of this car
    public float carSpeed;

    // Called at every physics steps
    public void FixedUpdate()
    {
        // We get the inputs
        carSpeed = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (GameObject wheel in listOfWheels)
        {
            if (wheel.GetComponent<WheelsScript>().steering)
            {
                wheel.GetComponent<WheelCollider>().steerAngle = steering;
            }
            if (wheel.GetComponent<WheelsScript>().attachedToMotor)
            {
                wheel.GetComponent<WheelCollider>().motorTorque = carSpeed;
            }

        }
    }
}



