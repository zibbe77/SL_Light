using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

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
    private List<(DateTime arrival_time, DateTime departure_time)> timeList = new List<(DateTime arrival_time, DateTime departure_time)>();


    public void AddStopTime(DateTime arrival_time, DateTime departure_time)
    {
        // check if that time exist
        timeList.Add((arrival_time, departure_time));
    }

    public int GetWeight(DateTime currentTime)
    {
        // hitta den närmsta avgången 
        (DateTime arrival_time, DateTime departure_time) closest = FindClosestArrival(currentTime);

        // räkna ut tiden mellan den närmsta avgången och tiden nu
        int waitTime = (int)(closest.departure_time - currentTime).TotalMinutes;
        int travelTime = (int)(closest.arrival_time - closest.departure_time).TotalMinutes;

        // räkna ut tiden mellan avgången och ankomsten 

        if (0 > (int)(closest.arrival_time - closest.departure_time).TotalMinutes)
        {
            throw new InvalidOperationException("negativ vikt");
        }

        Debug.Log($"departure: {closest.departure_time} arrival: {closest.arrival_time} wait: {waitTime} travel: {travelTime}");

        return waitTime + travelTime;
    }

    public (DateTime arrival_time, DateTime departure_time) FindClosestArrival(DateTime target)
    {
        var future = timeList.Where(t => t.departure_time >= target);

        if (future.Any())
            return future.OrderBy(t => t.departure_time).First();

        // Inga avgångar kvar idag – ta första imorgon med +24h
        var next = timeList.OrderBy(t => t.departure_time).First();
        return (next.arrival_time.AddDays(1), next.departure_time.AddDays(1));
    }
}
