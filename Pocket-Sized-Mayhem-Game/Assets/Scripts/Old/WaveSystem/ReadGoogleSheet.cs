using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
public class ReadGoogleSheet : MonoBehaviour
{
    public List<EnemySpawner> spawnPoints;
    public List<float> timeline;
    void Awake()
    {
        timeline = new List<float>();
        StartCoroutine(ObtainSheetData());
    }

    IEnumerator ObtainSheetData()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://sheets.googleapis.com/v4/spreadsheets/1BlWVHx6zDaQH8JyUftqd8_McF2sZvZ3LgW8MH55LXNg/values/B1:Z?key=AIzaSyDzN9hCwGdDssrZMwovVEilpTuNH9ri7NA");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            
            string data = www.downloadHandler.text;
            TableValues tableData = JsonConvert.DeserializeObject<TableValues>(data);
            foreach (string s in tableData.Values[0])
            {
                timeline.Add(float.Parse(s));
            }
            for (int i = 1; i < tableData.Values.Length; i++)
            {
                if (spawnPoints[i-1] != null)
                {
                    spawnPoints[i-1].times = new List<float>();
                    spawnPoints[i-1].spawncount = new List<float>();
                    foreach (string s in tableData.Values[i])
                    {
                        spawnPoints[i - 1].spawncount.Add(float.Parse(s));
                    }
                    spawnPoints[i-1].times = timeline;
                }
            }
        }
    }
}

public class TableValues
{
    [JsonProperty("values")]
    public string[][] Values { get; set; }
}