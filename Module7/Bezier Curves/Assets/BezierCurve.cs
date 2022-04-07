using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    [Header("Add 3 points for Quadratic or 4 for Cubic")]
    public Vector3[] points;
    public float timeScale = .1f;

    private bool isQuad = true;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (points.Length == 3)
        {
            Debug.Log("Creating Quadratic Bezier Curve");
            isQuad = true;
        }
        else if (points.Length == 4)
        {
            Debug.Log("Creating Cubic Bezier Curve");
            isQuad = false;
        }
        else
        {
            Debug.Log("ERROR: Incorrect Amout of Points");
            this.enabled = false;
        }

        setLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime*timeScale;

        if (isQuad) transform.position = quadBezier(time);
        else transform.position = cubicBezier(time);

        if (time >= 1) time = 0;
    }

    Vector3 quadBezier(float t)
    {
        Vector3 pos = (Mathf.Pow((1 - t), 2) * points[0]) + (2 * (1 - t) * t * points[1]) + (Mathf.Pow(t, 2) * points[2]);

        return pos;
    }

    Vector3 cubicBezier(float t)
    {
        Vector3 pos = (Mathf.Pow((1 - t), 3) * points[0]) + (3 * Mathf.Pow((1 - t), 2) * t * points[1]) + (3 * (1 - t) * Mathf.Pow(t, 2) * points[2]) + (Mathf.Pow(t, 3) * points[3]);

        return pos;
    }

    void setLineRenderer()
    {
        LineRenderer line = GetComponent<LineRenderer>();

        line.positionCount = points.Length;
        line.SetPositions(points);
    }
}
