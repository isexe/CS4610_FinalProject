using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        foreach (Edge edge in Edges)
        {
            if (edge == e) return true;
        }
        return false;
    }

    public static bool operator ==(Triangle left, Triangle right)
    {
        return (left.A == right.A || left.A == right.B || left.A == right.C)
            && (left.B == right.A || left.B == right.B || left.B == right.C)
            && (left.C == right.A || left.C == right.B || left.C == right.C);
    }

    public static bool operator !=(Triangle left, Triangle right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj is Triangle t)
        {
            return this == t;
        }

        return false;
    }

    public bool Equals(Triangle t)
    {
        return this == t;
    }

    public override int GetHashCode()
    {
        return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
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

        if (dist <= radius) return true;
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
    public float CalculateCircumradius(Vector2 circumcenter)
    {
        float radius;

        radius = Mathf.Sqrt(Mathf.Pow((circumcenter.x - A.x), 2) + Mathf.Pow((circumcenter.y - A.y), 2));

        return radius;

    }
}

public class Triangulation
{
    // TODO Fix this shit
    // !Something causes a crash, don't run til this message gone
    // Adapted from pseudo-code here:
    //  https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
    public static void DelanuayTriangluation(Graph graph)
    {
        // List of triangles
        List<Triangle> triangulation = new List<Triangle>();

        // add super-triangle
        Triangle super = new Triangle(new Vector2(-1000, -1000), new Vector2(0, 1000), new Vector2(1000, -1000));
        triangulation.Add(super);

        foreach (Point point in graph.Points)
        {
            // empty badtriangles set
            List<Triangle> badTriangles = new List<Triangle>();
            // Create list of edges to represent polygon
            List<Edge> polygon = new List<Edge>();

            // foreach triangle in trianglesList
            foreach (Triangle triangle in triangulation)
            {
                // check if point is inside of circumcircle of triangle
                if (triangle.isPointInsideCircumcircle(point))
                {
                    Debug.Log("Found point inside circumcircle");
                    // if it is, add triangle to badTriangles
                    badTriangles.Add(triangle);
                }
            }




            // !ERROR is happening here
            // foreach triangle edge in badTriangles
            foreach (Triangle triangle in badTriangles)
            {
                foreach (Edge edge in triangle.Edges)
                {
                    // !This will never add edge since poly is empty
                    // add edge to poly if edge not in already
                    if (!polygon.Contains(edge)) polygon.Add(edge);
                }
                //remove triangle from triangleList
                triangulation.Remove(triangle);
            }

            // foreach edge in polygon
            foreach (Edge edge in polygon)
            {
                Debug.Log("Adding edge from polygon to triangulation");
                //newTriangle = triangle from edge to point
                Triangle newTriangle = new Triangle(edge.P1.Pos, edge.P2.Pos, point.Pos);

                //add newTriangle to triangulation
                triangulation.Add(newTriangle);
            }
        }
        // remove super from triangulation
        triangulation.Remove(super);

        // return triangulation...or...
        // foreach triangle in triangulation
        foreach (Triangle triangle in triangulation)
        {
            //foreach edge in triangle add edge to graph
            foreach (Edge edge in triangle.Edges)
            {
                Debug.Log("There were edges");
                graph.Edges.Add(edge);
            }
        }
    }
}
