using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Security.Cryptography;
using System;

public class HandleStops : MonoBehaviour
{
    [SerializeField] GameObject stopPrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] float spaceMultiplier;

    private string stopsFile;

    private Graph graph = new Graph();

    void Start()
    {
        stopsFile = Application.dataPath + "/Files/sl_stops.txt";

        HandleTempData handleTempData = new HandleTempData();
        handleTempData.ReadIn();

        CreatStops();
        PopGraph(handleTempData);
    }

    void CreatStops()
    {
        using (var reader = new StreamReader(stopsFile))
        {
            // skip first line
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                Stop stop = new(
                    int.Parse(values[0]),
                    values[1],
                    double.Parse(values[2], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(values[3], System.Globalization.CultureInfo.InvariantCulture),
                    values[4]
                );
                graph.Add(stop);

                GameObject stopObj = Instantiate(stopPrefab, new Vector3((float)stop.lon * spaceMultiplier, 0, (float)stop.lat * spaceMultiplier), Quaternion.identity);
                stopObj.name = stop.name;
                TMP_Text TMP = stopObj.transform.GetChild(0).gameObject.GetComponentInChildren<TMP_Text>();
                TMP.text = stop.name;

            }
        }
    }
    void PopGraph(HandleTempData handleTempData)
    {
        Dictionary<string, TempRoute>.ValueCollection valueCollRoute = handleTempData.tempRoutes.Values;

        foreach (TempRoute route in valueCollRoute)
        {
            Dictionary<string, TempTrip>.ValueCollection valueCollTrip = route.tempTrips.Values;

            // Creat Line obj
            GameObject lineObj = Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            lineObj.name = route.route_id + " | " + route.route_short_name + " | " + route.route_long_name;
            LineRenderer lr = lineObj.GetComponent<LineRenderer>();

            // Fineds the longest trip on the line. Most likly to have the entire line. Only for visual
            int longesTrip = 0;
            int longesTripIndex = 0;
            int index = 0;

            foreach (TempTrip trip in valueCollTrip)
            {
                if (trip.stops.Count > longesTrip)
                {
                    longesTrip = trip.stops.Count;
                    longesTripIndex = index;
                }

                index++;
            }

            // Puts in graph and draws the points
            int currentTripIndex = 0;

            foreach (TempTrip trip in valueCollTrip)
            {
                for (int i = 1; i < trip.stops.Count; i++)
                {
                    DateTime arrival_time = ParseGtfsTime(trip.stops[i].arrival_time);
                    DateTime departure_time = ParseGtfsTime(trip.stops[i - 1].departure_time);

                    double trip_id;
                    bool success = double.TryParse(trip.stops[i].trip_id, out trip_id);
                    if (success)
                    {
                        graph.Connect(trip.stops[i - 1].stop_id, trip.stops[i].stop_id, trip_id, arrival_time, departure_time);
                    }
                    else
                    {
                        print("Fail -> " + trip.stops[i].trip_id);
                    }
                }
                for (int i = 0; i < trip.stops.Count; i++)
                {
                    if (currentTripIndex == longesTripIndex)
                    {
                        // Debug.Log(route.route_id + " | " + route.route_short_name + " | " + route.route_long_name + "-->" + longesTripIndex);

                        lr.positionCount = lr.positionCount + 1;
                        Stop stop = graph.GetStopFromId(trip.stops[i].stop_id);
                        lr.SetPosition(i, new Vector3((float)stop.lon * spaceMultiplier, 0, (float)stop.lat * spaceMultiplier));
                    }
                }

                currentTripIndex++;
            }

        }
    }
    private DateTime ParseGtfsTime(string timeStr)
    {
        var parts = timeStr.Split(':');
        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        int seconds = int.Parse(parts[2]);

        // Lägg till extra timmar på referensdatumet om hours >= 24

        DateTime dateTime = new DateTime();

        dateTime.Date
        .AddHours(hours)
        .AddMinutes(minutes)
        .AddSeconds(seconds);

        return dateTime;
    }
}
