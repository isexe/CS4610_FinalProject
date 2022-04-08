using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierClass : MonoBehaviour
{
    public Transform point1, point2, point3, point4;

    private bool isCubic = true;

    Vector3 p0, p1, p2, p3;
    float u = 0;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        Reset_t();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCubic)
        {
            point3.gameObject.SetActive(false);
        }
        else
        {
            point3.gameObject.SetActive(true);
        }

        if (u < 1f)
        {
            u += .001f;
            Vector3 tempPos;


            if (isCubic)
            {
                tempPos = (Mathf.Pow((1 - u), 3) * p0) + (3 * Mathf.Pow((1 - u), 2) * u * p1) + (3 * (1 - u) * Mathf.Pow(u, 2) * p2) + (Mathf.Pow(u, 3) * p3);
            }
            else
            {
                tempPos = (Mathf.Pow((1 - u), 2) * p0) + (2 * (1 - u) * u * p1) + (Mathf.Pow(u, 2) * p3);
            }
            
            transform.position = tempPos;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, tempPos);
        }
    }

    public void Reset_t()
    {
        p0 = point1.position;
        p1 = point2.position;
        p2 = point3.position;
        p3 = point4.position;
        u = 0;
        line.positionCount = 0;
    }

    public void ChangeMode()
    {
        if(u >= 1f)
        {
            isCubic = !isCubic;
        }
        
    }
}
