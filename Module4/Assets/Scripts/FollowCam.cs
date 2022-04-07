using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // Variables
    public Transform targetObj;

    Vector3 iniOffset;
    Vector3 camPos;

    // Start is called before the first frame update
    void Start()
    {
        iniOffset = transform.position - targetObj.position;
    }

    // Update is called once per frame
    void Update()
    {
        camPos = iniOffset + targetObj.position;
        //transform.position = camPos;

        transform.position = Vector3.Lerp(transform.position, camPos, Time.deltaTime);
    }
}
