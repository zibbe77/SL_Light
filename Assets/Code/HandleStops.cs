using UnityEngine;
using System.IO;
using TMPro;

public class HandleStops : MonoBehaviour
{
    [SerializeField] GameObject stopPrefab;
    [SerializeField] float spaceMultiplier;

    private string stopsFile;

    void Start()
    {
        stopsFile = Application.dataPath + "/Files/sl_stops.txt";
        HandleData();
    }

    void HandleData()
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


                //GameObject stopObj = Instantiate(stopPrefab, new Vector3((float)stop.lat * spaceMultiplier, 0, (float)stop.lon * spaceMultiplier), Quaternion.identity);
                GameObject stopObj = Instantiate(stopPrefab, new Vector3((float)stop.lon * spaceMultiplier, 0, (float)stop.lat * spaceMultiplier), Quaternion.identity);
                stopObj.name = stop.name;
                TMP_Text TMP = stopObj.transform.GetChild(0).gameObject.GetComponentInChildren<TMP_Text>();
                TMP.text = stop.name;
                // TMP.SetText(stop.name);

                // Debug.Log(stop.stop_name);


            }
        }
    }
}
