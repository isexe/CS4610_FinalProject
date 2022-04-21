using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Point
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2 Pos
    {
        get
        {
            return new Vector2(X, Y);
        }
        set
        {
            X = value.x;
            Y = value.y;
        }
    }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Point(Vector2 pos)
    {
        X = pos.x;
        Y = pos.y;
    }

    public float DistanceToPoint(Point other)
    {
        float dist = Mathf.Sqrt(Mathf.Pow((X - other.X), 2) + Mathf.Pow((Y - other.Y), 2));
        return dist;
    }

    public float DistanceToPosition(Vector2 position)
    {
        return DistanceToPoint(new Point(position));
    }

    public bool Equals(Point other)
    {
        if (this.X.Equals(other.X) && this.Y.Equals(other.Y)) return true;
        return false;
    }
}

public class Edge
{
    public Point P1 { get; set; }
    public Point P2 { get; set; }

    public Edge(Point p1, Point p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public Edge(Vector2 p1, Vector2 p2)
    {
        P1 = new Point(p1);
        P2 = new Point(p2);
    }

    public float Weight
    {
        get
        {
            return CalculateWeight();
        }
    }

    float CalculateWeight()
    {
        float value = P1.DistanceToPoint(P2);
        return value;
    }

    public bool Equals(Edge other)
    {
        if (this.P1.Equals(other.P1) && this.P2.Equals(other.P2)) return true;
        return false;
    }
}

public class Triangle
{
    public List<Vector2> Vertices { get; set; }
    public List<Edge> Edges { get; }

    // Used for calculations
    private Vector2 A;
    private Vector2 B;
    private Vector2 C;

    public Triangle(Vector2 v1, Vector2 v2, Vector2 v3)
    {
        Vertices = new List<Vector2>(3);
        Edges = new List<Edge>(3);

        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);

        Edges.Add(new Edge(v1, v2));
        Edges.Add(new Edge(v2, v3));
        Edges.Add(new Edge(v3, v1));

        A = v1;
        B = v2;
        C = v3;
    }

    public bool ContainsEdge(Edge e)
    {
        foreach(Edge edge in Edges)
        {
            if (edge.Equals(e)) return true;
        }
        return false;
    }

    // Detects if p is contained within the circumcircle of this triangle
    public bool isPointInsideCircumcircle(Point p)
    {
        // Calculate Circumcenter *WORKS
        Vector2 center = CalculateCircumcenter();

        // Calculate circumradius
        // Ignore complex equation, just gonna find dist from center to vertex
        float radius = CalculateCircumradius(center);

        // Check if p distance is < than radius from center
        float dist = p.DistanceToPosition(center);

        if (dist < radius) return true;
        return false;
    }

    // Equation from:
    //  https://en.wikipedia.org/wiki/Circumscribed_circle#Circumcircle_equations
    public Vector2 CalculateCircumcenter()
    {
        float D = 2 * (A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y));

        Vector2 U = new Vector2();
        U.x = (1 / D) * ((Mathf.Pow(A.x, 2) + Mathf.Pow(A.y, 2)) * (B.y - C.y) + (Mathf.Pow(B.x, 2) + Mathf.Pow(B.y, 2)) * (C.y - A.y) + (Mathf.Pow(C.x, 2) + Mathf.Pow(C.y, 2)) * (A.y - B.y));
        U.y = (1 / D) * ((Mathf.Pow(A.x, 2) + Mathf.Pow(A.y, 2)) * (C.x - B.x) + (Mathf.Pow(B.x, 2) + Mathf.Pow(B.y, 2)) * (A.x - C.x) + (Mathf.Pow(C.x, 2) + Mathf.Pow(C.y, 2)) * (B.x - A.x));

        return U;
    }

    // Currently just using distance function
    // May try linked equation to see if better
    //  https://www.mathopenref.com/trianglecircumcircle.html
    public float CalculateCircumradius(Vector2 circumcenter) {
        float radius;

        radius = Mathf.Sqrt(Mathf.Pow((circumcenter.x - A.x), 2) + Mathf.Pow((circumcenter.y - A.y), 2));

        return radius;

    }
}

public class Graph
{
    public List<Point> Points { get; set; }
    public List<Edge> Edges { get; set; }

    public Graph()
    {
        Points = new List<Point>();
        Edges = new List<Edge>();
    }

    public void GenerateDemoEdges()
    {
        Edges.Clear();
        foreach (Point p1 in Points)
        {
            foreach (Point p2 in Points)
            {
                Edge e = new Edge(p1, p2);
                Edges.Add(e);
            }
        }
    }

    // TODO Fix this shit
    // Adapted from pseudo-code here:
    //  https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
    public void DelanuayTriangluation()
    {
        // List of triangles
        List<Triangle> triangulation = new List<Triangle>();

        // add super-triangle
        Triangle super = new Triangle(new Vector2(-1000, -1000), new Vector2(0, 1000), new Vector2(1000, -1000));
        triangulation.Add(super);

        foreach(Point point in Points)
        {
            // empty badtriangles set
            List<Triangle> badTriangles = new List<Triangle>();

            // foreach triangle in trianglesList
            foreach(Triangle triangle in triangulation)
            {
                // check if point is inside of circumcircle of triangle
                if (triangle.isPointInsideCircumcircle(point))
                {
                    Debug.Log("Found point inside circumcircle");
                    // if it is, add triangle to badTriangles
                    badTriangles.Add(triangle);
                }
            }
            // Create list of edges to represent polygon
            List<Edge> polygon = new List<Edge>();


            // !ERROR is happening here
            // foreach triangle edge in badTriangles
            foreach(Triangle triangle in badTriangles)
            {
                foreach(Edge edge in triangle.Edges)
                {
                    bool notShared = true;
                    // if edge not shared by other triangle in badtriangle add edge to polygon
                    foreach(Triangle badTriangle in badTriangles)
                    {
                        Debug.Log("looking for shared edges");
                        if (badTriangle.ContainsEdge(edge)) notShared = false;
                    }
                    if (notShared)
                    {
                        Debug.Log("found edge that isn't shared");
                        polygon.Add(edge);
                    }
                }
            }            
            
            // foreach triangle in badTriangle
            foreach(Triangle triangle in badTriangles)
            {
                //remove triangle from triangleList
                triangulation.Remove(triangle);
            }
                
            // foreach edge in polygon
            foreach(Edge edge in polygon)
            {
                Debug.Log("Adding edge from polygon to triangulation");
                //newTriangle = triangle from edge to point
                Triangle newTriangle = new Triangle(edge.P1.Pos, edge.P2.Pos, point.Pos);

                //add newTriangle to triangulation
                triangulation.Add(newTriangle);
            }
        }
        // foreach triangle in triangulation
        foreach(Triangle triangle in triangulation)
        {
            //if appart of super triangle remove
            if (triangle.Equals(super))
            {
                triangulation.Remove(triangle);
                break;
            }
        }
        // return triangulation...or...
        // foreach triangle in triangulation
        foreach(Triangle triangle in triangulation)
        {
            //foreach edge in triangle add edge to graph
            foreach(Edge edge in triangle.Edges)
            {
                Debug.Log("There were edges");
                Edges.Add(edge);
            }
        }
    }

    public void AddPoint(Vector2 point)
    {
        Points.Add(new Point(point.x, point.y));
    }

    public void AddPoint(Vector2Int point)
    {
        AddPoint(new Vector2(point.x, point.y));
    }
}
