using System;
using System.Collections.Generic;

public class Edge
{
    public Edge(double trip_id, Stop connectedNode)
    {
        this.connectedNode = connectedNode;
        this.trip_id = trip_id;
    }
    public Edge(double trip_id, DateTime arrival_time, DateTime departure_time, Stop connectedNode)
    {
        this.connectedNode = connectedNode;
        this.trip_id = trip_id;
        timeList.Add((arrival_time, departure_time));
    }
    public Stop connectedNode { get; }
    public double trip_id { get; }
    private List<(DateTime, DateTime)> timeList = new List<(DateTime arrival_time, DateTime departure_time)>();


    public void AddStopTime(DateTime arrival_time, DateTime departure_time)
    {
        // check if that time exist
        timeList.Add((arrival_time, departure_time));
    }
    public int GetWeight()
    {
        return 1;
    }

    public int GetHeuristic()
    {
        return 1;
    }

}
