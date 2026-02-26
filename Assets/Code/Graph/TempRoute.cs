using System;
using System.Collections.Generic;

class TempRoute
{
    public TempRoute(string route_id, string agency_id, string route_short_name, string route_long_name, string route_type, string route_url)
    {
        this.route_id = route_id;
        this.agency_id = agency_id;
        this.route_short_name = route_short_name;
        this.route_long_name = route_long_name;
        this.route_type = route_type;
        this.route_url = route_url;
    }

    public string route_id { get; }
    public string agency_id { get; }
    public string route_short_name { get; }
    public string route_long_name { get; }
    public string route_type { get; }
    public string route_url { get; }

    public Dictionary<string, TempTrip> tempTrips = new Dictionary<string, TempTrip>();

}