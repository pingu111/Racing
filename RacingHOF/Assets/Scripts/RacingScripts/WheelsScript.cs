using UnityEngine;
using System.Collections;

public class WheelsScript : MonoBehaviour {

    /// is this wheel left or right ?
    [SerializeField]
    public bool isLeft;

    /// is this wheel attached to motor?
    [SerializeField]
    public bool attachedToMotor;

    /// does the steer apply to this wheel ?
    [SerializeField]
    public bool steering;

	// Update is called once per frame
	void Update ()
    {
        if (isLeft)
        {
            // Do the rotation of the visuals wheels
        }
        else
        {
            // Do the rotation of the visuals wheels
        }
    }
}
 