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

    public override bool Equals(object obj)
    {
        if(obj is Point p)
        {
            return this == p;
        }

        return false;
    }

    public bool Equals(Point other)
    {
        if (this.X.Equals(other.X) && this.Y.Equals(other.Y)) return true;
        return false;
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static bool operator !=(Point left, Point right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
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

    public static bool operator ==(Edge left, Edge right)
    {
        return (left.P1 == right.P1 || left.P1 == right.P2)
            && (left.P2 == right.P1 || left.P2 == right.P2);
    }

    public static bool operator !=(Edge left, Edge right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if(obj is Edge e)
        {
            return this == e;
        }

        return false;
    }

    public bool Equals(Edge e)
    {
        return this == e;
    }

    public override int GetHashCode()
    {
        return P1.GetHashCode() ^ P2.GetHashCode();
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
                if(p1 != p2)
                {
                    Edge e = new Edge(p1, p2);
                    if(!Edges.Contains(e)) Edges.Add(e);
                }
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

    public void AddPoint(Point p)
    {
        Points.Add(p);
    }
}
