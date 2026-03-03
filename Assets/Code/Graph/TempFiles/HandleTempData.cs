using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class HandleTempData
{

    public Dictionary<string, TempRoute> tempRoutes = new Dictionary<string, TempRoute>();

    // first string is tripId seckend is route_id 
    public Dictionary<string, string> tripIdLookUp = new Dictionary<string, string>();
    String routeFile;
    String tripFile;
    String slStopFile;

    public void ReadIn()
    {
        routeFile = Application.dataPath + "/Files/sl_routes.txt";
        tripFile = Application.dataPath + "/Files/sl_trips.txt";
        slStopFile = Application.dataPath + "/Files/sl_stop_times.txt";

        ReadInRoutes();
        ReadInTrips();
        ReadInStopTimes();
    }

    void ReadInRoutes()
    {
        using (var reader = new StreamReader(routeFile))
        {
            // skip first line
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                TempRoute tempRoute = new TempRoute(
                    values[0],
                    values[1],
                    values[2],
                    values[3],
                    values[4],
                    values[5]
                );

                tempRoutes.Add(tempRoute.route_id, tempRoute);
            }
        }
    }
    void ReadInTrips()
    {
        using (var reader = new StreamReader(tripFile))
        {
            // skip first line
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                TempTrip tempTrip = new TempTrip(
                    values[0],
                    values[1],
                    values[2],
                    values[3],
                    values[4]
                );

                TempRoute tempRoute = tempRoutes[tempTrip.route_id];
                tempRoute.tempTrips.Add(tempTrip.trip_id, tempTrip);

                tripIdLookUp.Add(tempTrip.trip_id, tempTrip.route_id);

            }
        }
    }
    void ReadInStopTimes()
    {
        using (var reader = new StreamReader(slStopFile))
        {
            // skip first line
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');

                TempSlStopTimes tempSlStopTimes = new TempSlStopTimes(
                    values[0],
                    values[1],
                    values[2],
                    values[3],
                    values[4],
                    values[5],
                    values[6]
                );

                TempRoute tempRoute = tempRoutes[tripIdLookUp[tempSlStopTimes.trip_id]];
                TempTrip tempTrip = tempRoute.tempTrips[tempSlStopTimes.trip_id];
                tempTrip.stops.Add(tempSlStopTimes);
            }
        }
    }
}