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

        SetCurrentTime("11:00:05");
        // Submit();
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
                stopObj.name = stop.name + " |" + stop.id;
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
                        if (graph.GetEdgeBetween(trip.stops[i - 1].stop_id, trip.stops[i].stop_id) != null)
                        {
                            GameObject lineObj = Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                            lr.positionCount = 2;

                            Stop stop = graph.GetStopFromId(trip.stops[i - 1].stop_id);
                            lr.SetPosition(0, new Vector3((float)stop.lon * spaceMultiplier, 0, (float)stop.lat * spaceMultiplier));

                            Stop stop2 = graph.GetStopFromId(trip.stops[i].stop_id);
                            lr.SetPosition(1, new Vector3((float)stop2.lon * spaceMultiplier, 0, (float)stop2.lat * spaceMultiplier));
                        }

                        graph.Connect(trip.stops[i - 1].stop_id, trip.stops[i].stop_id, trip_id, arrival_time, departure_time, double.Parse(route.route_id));
                    }
                    else
                    {
                        print("Fail -> " + trip.stops[i].trip_id);
                    }
                }
            }
        }
    }
    int from = 740021696;
    int to = 740011606;

    DateTime currentTime;

    public void SetFrom(string s)
    {
        bool success = int.TryParse(s, out from);
        if (success != true)
        {
            Debug.LogWarning("not a number");
        }
    }
    public void SetTo(string s)
    {
        bool success = int.TryParse(s, out to);
        if (success != true)
        {
            Debug.LogWarning("not a number");
        }
    }
    public void SetCurrentTime(string s)
    {
        currentTime = ParseGtfsTime(s);
        Debug.Log(currentTime);
    }
    public void Submit()
    {
        List<(Stop stop, int time)> stops = graph.GetPath(from, to, currentTime);

        Debug.Log("----------------");
        Debug.Log("Amount" + stops.Count);
        Debug.Log("----------------");
        foreach ((Stop stop, int time) s in stops)
        {
            Debug.Log($"{s.stop.name} --> {currentTime.AddMinutes(s.time):HH:mm} ");
        }
        Debug.Log("----------------");
    }


    private DateTime ParseGtfsTime(string timeStr)
    {
        var parts = timeStr.Split(':');
        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        int seconds = int.Parse(parts[2]);

        // Lägg till extra timmar på referensdatumet om hours >= 24

        return new DateTime().AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
    }
}
