using System.Collections.Generic;


class TempTrip
{
    public TempTrip(string route_id, string service_id, string trip_id, string trip_headsign, string trip_short_name)
    {
        this.route_id = route_id;
        this.service_id = service_id;
        this.trip_id = trip_id;
        this.trip_headsign = trip_headsign;
        this.trip_short_name = trip_short_name;
    }

    public string route_id { get; }
    public string service_id { get; }
    public string trip_id { get; }
    public string trip_headsign { get; }
    public string trip_short_name { get; }

    public List<TempSlStopTimes> stops = new List<TempSlStopTimes>();
}