using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Graph
{
    private Dictionary<Stop, List<Edge>> adjacencyList = new Dictionary<Stop, List<Edge>>();

    #region Public Funtions 
    public void Add(Stop node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList.Add(node, new List<Edge>());
        }
    }
    public void Connect(string from, string to, double trip_id, DateTime arrival_time, DateTime departure_time)
    {
        Connect(GetStopFromId(from), GetStopFromId(to), trip_id, arrival_time, departure_time);
    }

    public void Connect(Stop from, Stop to, double trip_id, DateTime arrival_time, DateTime departure_time)
    {
        CheckAdjListContains(from, to);
        Edge edge = GetEdgeBetween(from, to);
        if (edge != null)
        {
            edge.AddStopTime(arrival_time, departure_time);
        }
        else
        {
            adjacencyList[from].Add(new Edge(trip_id, arrival_time, departure_time, to));
        }
    }
    public List<Stop> GetNodes()
    {
        List<Stop> nodes = new List<Stop>();

        foreach (var pair in adjacencyList)
        {
            nodes.Add(pair.Key);
        }
        return nodes;
    }
    public List<Edge> GetEdgesFrom(Stop stop)
    {
        return adjacencyList[stop];
    }
    public Edge GetEdgeBetween(Stop from, Stop to)
    {
        CheckAdjListContains(from, to);

        foreach (Edge edge in adjacencyList[from])
        {
            if (edge.connectedNode.Equals(to))
            {
                return edge;
            }
        }

        return null;
    }

    public List<Stop> GetPath(Stop from, Stop to, DateTime startTime)
    {
        if (!adjacencyList.ContainsKey(from) || !adjacencyList.ContainsKey(to)) { return null; }

        // PriorityQueue fins tydligen inte i unity. Skulle använda en PriorityQueue om jag kunde. Id för att fixa ties.
        var workingQueue = new SortedList<(int cost, int id), Stop>();
        int id = 0;

        var nodeData = new Dictionary<Stop, (int pathCost, Stop parent)>();
        workingQueue.Add((0 + GetHeuristic(from, to), ++id), from);

        Stop currentStop = null;
        while (workingQueue.Count < 0)
        {
            Stop parent = currentStop;
            currentStop = workingQueue.Values[0];

            nodeData.Add(currentStop, (workingQueue.Keys[0].cost, parent));
            workingQueue.RemoveAt(0);

            if (currentStop.Equals(to))
            {
                var list = new List<Stop>();
                list.Add(currentStop);

                while (true)
                {
                    currentStop = nodeData[currentStop].parent;
                    if (currentStop == null)
                    {
                        return list;
                    }

                    list.Add(currentStop);
                }
            }

            List<Edge> edges = GetEdgesFrom(currentStop);
            foreach (Edge edge in edges)
            {
                // checks for better paths
                int pathCostCalc = nodeData[currentStop].pathCost + edge.GetWeight(startTime.AddMinutes(nodeData[currentStop].pathCost));

                if (nodeData.ContainsKey(edge.connectedNode))
                {
                    if (nodeData[edge.connectedNode].pathCost > pathCostCalc && nodeData[edge.connectedNode].parent != null)
                    {
                        nodeData[edge.connectedNode] = new(pathCostCalc, currentStop);
                    }
                }
                else
                {
                    workingQueue.Add((pathCostCalc + nodeData[currentStop].pathCost + GetHeuristic(currentStop, to), ++id), edge.connectedNode);
                }
            }
        }

        return null;
    }

    public Stop GetStopFromId(string stopId)
    {
        int stopIdNum;
        bool success = int.TryParse(stopId, out stopIdNum);
        if (!success)
        {
            throw new Exception("not a number");
        }

        foreach (Stop item in adjacencyList.Keys)
        {
            if (stopIdNum == item.id)
            {
                return item;
            }
        }

        return null;
    }

    public Stop GetStopFromId(int stopId)
    {
        foreach (Stop item in adjacencyList.Keys)
        {
            if (stopId == item.id)
            {
                return item;
            }
        }

        return null;
    }

    #endregion
    #region Helpers

    // Shit Heuristic
    private int GetHeuristic(Stop current, Stop goal)
    {
        const double Earth_Radius_Km = 6371;
        const double SL_MaxSpeed = 80.0;


        double dLat = (goal.lat - current.lat) * Math.PI / 180;
        double dLon = (goal.lon - current.lon) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(current.lat * Math.PI / 180) * Math.Cos(goal.lat * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double km = Earth_Radius_Km * c; // distans i km

        return (int)math.ceil((km / SL_MaxSpeed) * 60);
    }
    private void CheckAdjListContains(Stop node1)
    {
        if (!adjacencyList.ContainsKey(node1))
        {
            throw new InvalidOperationException("One node do not exist");
        }
    }
    private void CheckAdjListContains(Stop node1, Stop node2)
    {
        if (!adjacencyList.ContainsKey(node1) || !adjacencyList.ContainsKey(node2))
        {
            throw new InvalidOperationException("One node do not exist");
        }
    }
    #endregion
}
