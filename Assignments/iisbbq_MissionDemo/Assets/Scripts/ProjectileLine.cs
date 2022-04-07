using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLine : MonoBehaviour
{
    static public ProjectileLine S;

    [Header("Set in Inspector")]
    public float minDist = 0.1f;

    private LineRenderer line;
    private GameObject _poi;
    private List<Vector3> points;

    private LineRenderer previousLine;

    private void Awake()
    {
        S = this;

        // Get a reference to the LineRenderer
        line = GetComponent<LineRenderer>();
        // Disable the LineRenderer until it's needed
        line.enabled = false;
        // Initialize the points List
        points = new List<Vector3>();

        previousLine = GameObject.Find("PreviousProjectileLine").GetComponent<LineRenderer>();
        Material plMatTemp = previousLine.GetComponent<Renderer>().material;
        Color plColTemp = Color.white;
        plMatTemp.color = plColTemp;

        previousLine.enabled = false;
    }

    // This is a property
    public GameObject poi
    {
        get
        {
            return (_poi);
        }
        set
        {
            _poi = value;
            if (_poi != null)
            {
                previousLine.positionCount = points.Count;
                for(int i=0; i<points.Count; i++)
                {
                    previousLine.SetPosition(i, points[i]);
                }
                previousLine.enabled = true;

                // When _poi is set to something new, it resets everthing
                line.enabled = false;
                points = new List<Vector3>();

                AddPoint();
            }
        }
    }

    // Clears line directly
    public void Clear()
    {
        _poi = null;

        Vector3[] emptyPos = { };

        // Reset Line
        line.SetPositions(emptyPos);
        line.enabled = false;
        points = new List<Vector3>();

        // Reset Prev Line
        previousLine.SetPositions(emptyPos);
        previousLine.enabled = false;
    }

    public void AddPoint()
    {
        // This is called to add a point to the line
        Vector3 pt = _poi.transform.position;
        if (points.Count > 0 && (pt - lastPoint).magnitude < minDist)
        {
            // If the point isn't far enough from the last point, it returns
            return;
        }
        if (points.Count == 0) // if this is the launch point
        {
            Vector3 launchPosDiff = pt - Slingshot.LAUNCH_POS;

            // adds an line for aiming
            points.Add(pt + launchPosDiff);
            points.Add(pt);
            line.positionCount = 2;
            // sets the first two points
            line.SetPosition(0, points[0]);
            line.SetPosition(1, points[1]);
            //Enables the line renderer
            line.enabled = true;
        }
        else
        {
            //Normal behavior of adding points
            points.Add(pt);
            line.positionCount = points.Count;
            line.SetPosition(points.Count - 1, lastPoint);
            line.enabled = true;
        }
    }

    public Vector3 lastPoint
    {
        get
        {
            if (points == null)
            {
                // If there are no points, returns Vector3.zero
                return Vector3.zero;
            }
            return (points[points.Count - 1]);
        }
    }

    private void FixedUpdate()
    {
        if (poi == null)
        {
            //If there is no poi, search for one
            if (FollowCam.POI != null)
            {
                if (FollowCam.POI.tag == "Projectile")
                {
                    poi = FollowCam.POI;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        AddPoint();
        if (FollowCam.POI == null)
        {
            poi = null;
        }
    }
}
