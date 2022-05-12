using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rifle_control : MonoBehaviour
{
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    

    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.Find("PlayerCamera");
        if (Input.GetMouseButtonDown(0))
        {  
            Instantiate(bullet, transform.position + new Vector3(0, .11f, 0), transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 10);
        
        }
        // transform.Rotate((Input.GetAxis("Mouse Y") * 2.3f), 0, 0, Space.Self);

        // Should add something to check for max rotation so weapon doesn't do flips
        transform.rotation = Quaternion.Euler(-Camera.main.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
