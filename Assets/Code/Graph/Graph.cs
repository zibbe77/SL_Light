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
    public List<Stop> GetPath(int from, int to, DateTime startTime)
    {
        return GetPath(GetStopFromId(from), GetStopFromId(to), startTime);
    }
    public List<Stop> GetPath(Stop from, Stop to, DateTime startTime)
    {
        if (!adjacencyList.ContainsKey(from) || !adjacencyList.ContainsKey(to)) { return null; }
        Debug.Log("From: " + from.name + " To: " + to.name + "Time: " + startTime);


        // PriorityQueue fins tydligen inte i unity. Skulle använda en PriorityQueue om jag kunde. Id för att fixa ties.
        var workingQueue = new SortedList<(int cost, int id, int pathCost), (Stop stop, Stop parent)>();
        int id = 0;

        var nodeData = new Dictionary<Stop, (int pathCost, Stop parent)>();
        workingQueue.Add((0 + GetHeuristic(from, to), id++, 0), (from, null));

        Stop currentStop = null;

        while (workingQueue.Count > 0)
        {
            var current = workingQueue.Values[0];
            var currentKey = workingQueue.Keys[0];
            workingQueue.RemoveAt(0);

            currentStop = current.stop;
            Stop parent = current.parent;

            if (nodeData.ContainsKey(currentStop)) continue;
            nodeData.Add(currentStop, (currentKey.pathCost, parent));

            if (currentStop.Equals(to))
            {
                Debug.Log("<<<<<<<<<<<<<<<<<<");
                Debug.Log("End list");
                Debug.Log("<<<<<<<<<<<<<<<<<<");
                var list = new List<Stop>();
                var visited = new HashSet<Stop>();


                while (true)
                {
                    currentStop = nodeData[currentStop].parent;

                    if (currentStop == null)
                    {
                        list.Reverse();
                        return list;
                    }

                    if (visited.Contains(currentStop))
                    {
                        string chain = "";
                        foreach (var stop in list)
                        {
                            chain += stop.name + " -> ";
                        }
                        Debug.LogError($"Cykel! {currentStop.name} pekar tillbaka");
                        Debug.LogError($"Parent chain: {chain}");
                        return null;
                    }

                    Debug.Log(currentStop.name);

                    list.Add(currentStop);
                    visited.Add(currentStop);
                }

            }
            //Debug.Log("currentStop: " + currentStop.name);

            List<Edge> edges = GetEdgesFrom(currentStop);
            foreach (Edge edge in edges)
            {
                // Debug.Log(edge.trip_id);
                // checks for better paths

                int pathCostCalc = nodeData[currentStop].pathCost + edge.GetWeight(startTime.AddMinutes(nodeData[currentStop].pathCost));

                if (!nodeData.ContainsKey(edge.connectedNode))
                {
                    Debug.Log($"Kö: {edge.connectedNode.name} parent={currentStop.name} cost={pathCostCalc}");
                    workingQueue.Add((pathCostCalc + GetHeuristic(currentStop, to), id++, pathCostCalc), (edge.connectedNode, currentStop));
                }
            }
        }

        Debug.Log("return null");
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
        const double SL_Speed = 60;


        double dLat = (goal.lat - current.lat) * Math.PI / 180;
        double dLon = (goal.lon - current.lon) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(current.lat * Math.PI / 180) * Math.Cos(goal.lat * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double km = Earth_Radius_Km * c; // distans i km


        //Debug.Log("h: " + (int)math.ceil((km / SL_Speed) * 60));
        return (int)math.ceil((km / SL_Speed) * 60);
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
