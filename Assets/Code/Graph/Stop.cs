using System;
public class Stop
{
    public Stop(int stop_id, string stop_name, double stop_lat, double stop_lon, string location_typ)
    {
        id = stop_id;
        name = stop_name;
        lat = stop_lat;
        lon = stop_lon;
        this.location_typ = location_typ;
    }

    public int id { get; }
    public string name { get; }
    public double lat { get; }
    public double lon { get; }
    public string location_typ { get; }

    public override bool Equals(object obj)
    {
        if (obj is not Stop other) return false;
        if (ReferenceEquals(this, other)) return true;

        return id == other.id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id);
    }
}