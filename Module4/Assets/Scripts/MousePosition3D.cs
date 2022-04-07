using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition3D : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;    
    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 6;

        layerMask = ~layerMask;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if(Physics.Raycast(ray,out RaycastHit raycastHit, 100f, layerMask))
        {
            if (raycastHit.collider.name == "Plane")
            {
                transform.position = raycastHit.point;
                print(transform.position);
            }
        }
        if(Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Debug.DrawLine(ray.origin, hit.point);
        }
    }
}
