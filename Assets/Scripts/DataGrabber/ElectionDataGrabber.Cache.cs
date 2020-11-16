using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public partial class ElectionDataGrabber : MonoBehaviour
{
    private Dictionary<string, DistrictData> cachedElectionData;

    private void InitializeOrLoadCache()
    {
        if (cachedElectionData == null)
        {
            cachedElectionData = new Dictionary<string, DistrictData>();
            LoadItemCache();
        }
    }

    private void LoadItemCache()
    {
        string path = "Assets/Resources/cachedElectionData.json";
        if (!File.Exists(path))
        { 
            return;
        }

        using (FileStream fs = new FileStream(path, FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(fs))
            {
                string data = reader.ReadToEnd();
                cachedElectionData = JsonConvert.DeserializeObject<Dictionary<string, DistrictData>>(data);
                int sadas = 1;
            }
        }
    }

    private void SaveItemCache()
    {
        string path = "Assets/Resources/cachedElectionData.json";
        string data = JsonConvert.SerializeObject(cachedElectionData,Formatting.Indented);

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(data);
            }
        }

        UnityEditor.AssetDatabase.Refresh();
    }


    private void AddDataInCache(string url, DistrictData data)
    {
        InitializeOrLoadCache();

        if (cachedElectionData.ContainsKey(url))
            cachedElectionData[url] = data;
        else
            cachedElectionData.Add(url, data);

        SaveItemCache();
    }
}