using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KruskalMST
{
    public static Graph MST(Graph graph)
    {
        List<Edge> sortedEdges = new List<Edge>();
        foreach(Edge e in graph.Edges)
        {
            sortedEdges.Add(e);
        }
        sortedEdges.Sort((e1, e2) => e1.Weight.CompareTo(e2.Weight));

        foreach(Edge edge in sortedEdges)
        {
            Debug.Log("P1: (" + edge.P1.X + "," + edge.P1.Y + ")\n" + "P2: (" + edge.P2.X + "," + edge.P2.Y + ")");
            Debug.Log("Cost: " + edge.Weight);
        }
        Debug.Log("NumEdges: " + sortedEdges.Count);

        graph.Edges.Clear();

        // TODO implement so form of MST
        // reference:
        //  https://www.geeksforgeeks.org/kruskals-minimum-spanning-tree-algorithm-greedy-algo-2/
        // 1.) add smalled weighted edge
        // 2.) check for cycles
        // 3.) repeat 1-2 until there are V-1 edges
        
        // !This does not produce an MST, there is not cycle detection and just adds lowest weight to graph
        // !Currently just used to display somehting
        List<Point> foundPoints = new List<Point>();
        List<Point> donePoints = new List<Point>();

        foreach(Edge e in sortedEdges)
        {
            if (!donePoints.Contains(e.P1) && !donePoints.Contains(e.P2)){
                graph.Edges.Add(e);
                foundPoints.Add(e.P1);
                foundPoints.Add(e.P2);
            }

            if(graph.Edges.Count == graph.Points.Count-1)
            {
                break;
            }
            
            foreach(Point p in foundPoints)
            {
                List<Point> temp = foundPoints.FindAll(p1 => p1 == p);
                if(temp.Count >= 2)
                {
                    donePoints.Add(p);
                }
            }
        }

        return graph;
    }
}
