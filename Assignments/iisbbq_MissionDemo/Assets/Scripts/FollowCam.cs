using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static  public  GameObject  POI;  //static point of interest

    [Header("Set in Inspector")]
    public  float       easing = 0.05f;
    public  Vector2     minXY = Vector2.zero;

    [Header("Set Dynamically")]
    public  float       camZ; //the desired Z position of the camera

    void Awake () {
        camZ = this.transform.position.z;
    }

    void FixedUpdate() {
        //if (POI == null) return;
        //// gets the position of the POI
        //Vector3 destination = POI.transform.position;

        Vector3 destination;
        // If there is no POI, return to P: [0,0,0]
        if (POI == null)
        {
            destination = Vector3.zero;
        }
        else
        {
            // Get the position of the POI
            destination = POI.transform.position;
            // If poi is a projetile, check to see if it's at rest
            if (POI.tag == "Projectile")
            {
                // if it is sleeping (that is, not moving)
                if (POI.GetComponent<Rigidbody>().IsSleeping())
                {
                    // return to default view
                    POI = null;
                    return;
                }
            }
        }

        // limits the X & Y to minimum values
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        // Interpolate from the current camera position towards the destination
        destination = Vector3.Lerp(transform.position, destination, easing);
        // Force destination.z to be camZ to keep the camera away
        destination.z = camZ;
        // Set the camera to the destination
        transform.position = destination;
        // Set the orthographicSize of the camera to keep ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }
}
