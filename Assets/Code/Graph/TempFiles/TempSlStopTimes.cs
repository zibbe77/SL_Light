class TempSlStopTimes
{
    public TempSlStopTimes(string trip_id, string arrival_time, string departure_time, string stop_id, string stop_sequence, string pickup_type, string drop_off_type)
    {
        this.trip_id = trip_id;
        this.arrival_time = arrival_time;
        this.departure_time = departure_time;
        this.stop_id = stop_id;
        this.stop_sequence = stop_sequence;
        this.pickup_type = pickup_type;
        this.drop_off_type = drop_off_type;
    }
    public string trip_id { get; }
    public string arrival_time { get; }
    public string departure_time { get; }
    public string stop_id { get; }
    public string stop_sequence { get; }
    public string pickup_type { get; }
    public string drop_off_type { get; }
}