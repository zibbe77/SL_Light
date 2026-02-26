using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class HandleTempData : MonoBehaviour
{

    public Dictionary<string, TempRoute> tempRoutes = new Dictionary<string, TempRoute>();
    String routeFile;
    String tripFile;

    void Start()
    {
        routeFile = Application.dataPath + "/Files/sl_routes.txt";
        tripFile = Application.dataPath + "/Files/sl_trips.txt";

        ReadInRoutes();
        ReadInTrips();
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

            }
        }
    }
}